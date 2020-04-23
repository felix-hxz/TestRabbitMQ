using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using UtilsRabbitmq;

namespace TestRabbitmqConsumer
{

    public class Program
    {
        private const string QUEUE_NAME = "test_simple_queue";
        static void Main(string[] args)
        {
            //获取连接
            var connection = ConnectionUtils.GetConnection();
            //通过连接创建通道
            var channel = connection.CreateModel();
            //定义队列的消费者
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (ch, ea) =>
            {   

                try
                {
                    var msg = Encoding.UTF8.GetString(ea.Body.ToArray());
                    Console.WriteLine($"[consumer] msg:{msg}-----------------tagId:${ea.DeliveryTag}");
                }
                catch (Exception)
                {
                    throw new Exception();
                }
                finally
                {
                    channel.BasicAck(ea.DeliveryTag, false);
                }
            };

            //监听队列
            channel.BasicConsume(QUEUE_NAME, false, consumer);

            Console.ReadKey();
            
        }
    }
}
