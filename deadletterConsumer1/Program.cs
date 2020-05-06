using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;


namespace deadletterConsumer1
{
    class Program
    {    

        //死信队列  A
        private const string QUEUE_DEAD_LETTER_NAMEA = "deadLetterQueueA";
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

                    var properties = ea.BasicProperties;

                    properties.Headers.TryGetValue("exchange", out object value);

                    var values= Encoding.UTF8.GetString((byte[])value);

                    //JsonSerializer.Deserialize<object>();

                    var msg = Encoding.UTF8.GetString(ea.Body.ToArray());
                    Console.WriteLine($"[1] consumer  Body:{msg}   RoutingKey:{ea.RoutingKey}    Exchange:{ea.Exchange}   Properties:{value.ToString()}");
                    System.Threading.Thread.Sleep(1000);
               
                    //重新补发
                    channel.BasicPublish("test_businessExchange", "",false,null, ea.Body);


                }
                catch
                {

                }
                finally
                {
                   // channel.BasicAck(ea.DeliveryTag, false);
                }
            };
            //autoAck=false 关闭自动应答
            channel.BasicConsume(QUEUE_DEAD_LETTER_NAMEA, false, consumer);
            Console.ReadKey();
        }
    }
}
