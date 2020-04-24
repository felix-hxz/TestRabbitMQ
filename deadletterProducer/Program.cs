using System;
using RabbitMQ.Client;
using System.Text;
using System.Collections.Generic;

namespace deadletterProducer
{
    class Program
    {   

        //业务交换机
        private const string EXCHANGE_BUSINESS_NAME = "test_businessExchange";
        //死信交换机
        private const string DEAD_LETTER_EXCHANGE_NAME = "test_deadLetterExchange";
        //业务队列  A
        private const string QUEUE_BUSINESS_NAMEA = "businessQueueA";
        //业务队列  B
        private const string QUEUE_BUSINESS_NAMEB = "businessQueueB";
        //死信队列  A
        private const string QUEUE_DEAD_LETTER_NAMEA = "deadLetterQueueA";
        //死信队列  B
        private const string QUEUE_DEAD_LETTER_NAMEB = "deadLetterQueueB";


        static void Main(string[] args)
        {   
            using (var connection = UtilsRabbitmq.ConnectionUtils.GetConnection()) 
            {
                using (var channel = connection.CreateModel()) 
                {   

                    //声明业务队列A绑定交换机
                    channel.ExchangeDeclare(EXCHANGE_BUSINESS_NAME, ExchangeType.Fanout,true);
                    channel.QueueDeclare(QUEUE_BUSINESS_NAMEA,true,false,false,arguments:new Dictionary<string, Object> 
                    {
                        { "x-dead-letter-exchange",DEAD_LETTER_EXCHANGE_NAME },    //设置当前队列的DLX
                        { "x-dead-letter-routing-key","DEAD_LETTER_QUEUEA_ROUTING_KEY" }                     //设置当前DLK的路由key
                    });
                    channel.QueueBind(QUEUE_BUSINESS_NAMEA, EXCHANGE_BUSINESS_NAME,"");


                    //声明死信队列A绑定交换机
                    channel.ExchangeDeclare(DEAD_LETTER_EXCHANGE_NAME,ExchangeType.Direct,true);
                    channel.QueueDeclare(QUEUE_DEAD_LETTER_NAMEA,true,false,false);
                    channel.QueueBind(QUEUE_DEAD_LETTER_NAMEA, DEAD_LETTER_EXCHANGE_NAME, "DEAD_LETTER_QUEUEA_ROUTING_KEY");


                    for (int i = 0; i < 4; i++)
                    {
                        var msg = "deadletter";
                        channel.BasicPublish(EXCHANGE_BUSINESS_NAME, "", false, null, Encoding.UTF8.GetBytes(msg));

                        Console.WriteLine(msg);
                        System.Threading.Thread.Sleep(1000);
                    }

                    Console.ReadKey();
                }
            }
            
        }
    }
}
