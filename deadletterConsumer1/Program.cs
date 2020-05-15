using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;


namespace deadletterConsumer1
{
    class Program
    {    

        //死信队列  A
        private const string QUEUE_DEAD_LETTER_NAMEA = "fnlinker.dlx";
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

                    Console.WriteLine(properties.Headers);


                  /*  properties.Headers.TryGetValue("exchange", out object value);

                    var values= Encoding.UTF8.GetString((byte[])value);

                    //JsonSerializer.Deserialize<object>();

                    var msg = Encoding.UTF8.GetString(ea.Body.ToArray());
                    Console.WriteLine($"[2] consumer  Body:{msg}   RoutingKey:{ea.RoutingKey}    Exchange:{ea.Exchange}   Properties:{value.ToString()}");
                    System.Threading.Thread.Sleep(1000);


                    var properties1 = channel.CreateBasicProperties();
                    properties1.DeliveryMode = 2;   //消息持久化
                    properties1.ContentType = "application/json";
                    properties1.ContentEncoding = "UTF-8";

                    properties1.Headers = new Dictionary<string, object>
                        {
                            { "exchange","test_businessExchange"},
                            { "routingKey",""}
                        };
                    var msg1 = "deadletter";


                    //重新补发
                    channel.BasicPublish("test_businessExchange", "",false, properties1, ea.Body);
                    */

                }
                catch
                {

                }
                finally
                {
                   //channel.BasicAck(ea.DeliveryTag, false);
                }
            };
            //autoAck=false 关闭自动应答
            channel.BasicConsume(QUEUE_DEAD_LETTER_NAMEA, false, consumer);
            Console.ReadKey();
        }
    }
}
