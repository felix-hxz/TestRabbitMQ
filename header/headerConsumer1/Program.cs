using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace headerConsumer1
{
    class Program
    {
        private const string QUEUE_NAME = "test_queue_header_sms";
        private const string EXCHANGE_NAME = "test_exchange_header";
        static void Main(string[] args)
        {
            //获取连接
            var connection = UtilsRabbitmq.ConnectionUtils.GetConnection();
            //获取通道
            var channel = connection.CreateModel();
            //声明队列   exclusive是否独占队列 true该队列只能在当前连接只用  如果连接关闭就会自动删除
            channel.QueueDeclare(QUEUE_NAME, false, false, false);
            //队列绑定到交换机
            channel.QueueBind(QUEUE_NAME, EXCHANGE_NAME, "",arguments: new Dictionary<string, Object>
            {
                { "x-math","all"},
                { "info_type","sms"},
                { "error_type","email"},
            });
            //定义一个消费者
            var consumer = new EventingBasicConsumer(channel);
            //监听队列
            consumer.Received += (ch, ea) =>
            {
                try
                {
                    var msg = Encoding.UTF8.GetString(ea.Body.ToArray());
                    Console.WriteLine($"[2] consumer  msg:{msg}------------tag:${ea.ConsumerTag}");
                    System.Threading.Thread.Sleep(2000);
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
