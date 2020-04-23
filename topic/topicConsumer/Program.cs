using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace topicConsumer
{
    class Program
    {
        private const string QUEUE_NAME = "test_queue_topic_email";
        private const string EXCHANGE_NAME = "test_exchange_topic";
        static void Main(string[] args)
        {
            //获取连接
            var connection = UtilsRabbitmq.ConnectionUtils.GetConnection();
            //获取通道
            var channel = connection.CreateModel();
            //声明队列
            channel.QueueDeclare(QUEUE_NAME, false, false, false);
            //队列绑定到交换机
            channel.QueueBind(QUEUE_NAME, EXCHANGE_NAME, "good.add");
            channel.QueueBind(QUEUE_NAME, EXCHANGE_NAME, "good.delete");
            channel.QueueBind(QUEUE_NAME, EXCHANGE_NAME, "good.update");
            //定义一个消费者
            var consumer = new EventingBasicConsumer(channel);

            //监听队列
            consumer.Received += (ch, ea) =>
            {
                try
                {
                    var msg = Encoding.UTF8.GetString(ea.Body.ToArray());
                    Console.WriteLine("[1] consumer  msg:" + msg);
                    System.Threading.Thread.Sleep(1000);
                }
                catch
                {
                    throw new Exception();
                }
                finally
                {
                    //发送回执 手动确认消息已被消费
                    channel.BasicAck(ea.DeliveryTag, false);
                }
            };

            //autoAck=false 关闭自动应答
            channel.BasicConsume(QUEUE_NAME, false, consumer);
            Console.ReadKey();
        }
    }
}
