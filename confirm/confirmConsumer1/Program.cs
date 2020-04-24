using RabbitMQ.Client;
using System;
using System.Text;
using UtilsRabbitmq;
using RabbitMQ.Client.Events;

namespace confirmConsumer1
{
    class Program
    {
        private const string EXCHANGE_NAME = "test_confirm_exchange";
        private const string QUEUE_NAME = "test_confirm_queue1";
        static void Main(string[] args)
        {

            //获取连接
            var connection = ConnectionUtils.GetConnection();
            //声明通道
            var channel = connection.CreateModel();
            //声明队列
            channel.QueueDeclare(QUEUE_NAME, true,false,false);
            //绑定交换机
            channel.QueueBind(QUEUE_NAME, EXCHANGE_NAME, "info");
            //定义消费者
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
                catch (Exception)
                {

                }
                finally
                {
                    //channel.BasicAck(ea.DeliveryTag, true);
                }
            };

            //手动应答
            channel.BasicConsume(QUEUE_NAME, false, consumer);
            Console.ReadKey();
        }
    }
}
