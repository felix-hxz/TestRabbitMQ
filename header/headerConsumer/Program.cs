using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace headerConsumer
{
    class Program
    {   

        private const string QUEUE_NAME = "test_queue_header_email";
        private const string EXCHANGE_NAME = "test_exchange_header";

        static void Main(string[] args)
        {
            //获取连接
            var connection = UtilsRabbitmq.ConnectionUtils.GetConnection();
            //获取通道
            var channel = connection.CreateModel();
            //声明队列     bool durable = false是否持久化队列  mq重启后队列还存在
            channel.QueueDeclare(QUEUE_NAME, true, false, false);
            //队列绑定到交换机
            channel.QueueBind(QUEUE_NAME, EXCHANGE_NAME,"",arguments:new Dictionary<string, Object>
            {
                { "x-math","any"},
                { "info_type","sms"}
            });
            //定义一个消费者
            var consumer = new EventingBasicConsumer(channel);

            //监听队列
            consumer.Received += (ch, ea) =>
            {
                try
                {
                    var msg = Encoding.UTF8.GetString(ea.Body.ToArray());
                    Console.WriteLine($"[1] consumer  msg:{msg}------------tagId:{ea.DeliveryTag}");
                    System.Threading.Thread.Sleep(4000);
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
