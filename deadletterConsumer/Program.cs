using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace deadletterConsumer
{
    class Program
    {    

        //业务队列  A
        private const string QUEUE_BUSINESS_NAMEA = "businessQueueA";

        static void Main(string[] args)
        {
            //获取连接
            var connection = UtilsRabbitmq.ConnectionUtils.GetConnection();
            //获取通道
            var channel = connection.CreateModel();
            //定义一个消费者
            var consumer = new EventingBasicConsumer(channel);
            //监听队列
            consumer.Received += (ch, ea) =>
            {
                try
                {   
                    var msg = Encoding.UTF8.GetString(ea.Body.ToArray());
                    Console.WriteLine($"[1] consumer  Body:{msg}   RoutingKey:{ea.RoutingKey}    Exchange:{ea.Exchange}");
                    System.Threading.Thread.Sleep(1000);
                    throw new Exception();
                }
                catch
                {
                    channel.BasicNack(ea.DeliveryTag, false, false);
                }
                finally
                {
                    //发送回执  手动确认消息已被消费
                    //channel.BasicAck(ea.DeliveryTag, false);
                }
            };
            //autoAck=false 关闭自动应答
            channel.BasicConsume(QUEUE_BUSINESS_NAMEA, false, consumer);
            Console.ReadKey();
        }
    }
}
