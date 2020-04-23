using System;
using RabbitMQ.Client;
using System.Text;
using System.Collections.Generic;

namespace headerProducer
{

    class Program
    {

        private const string EXCHANGE_NAME = "test_exchange_header";

        static void Main(string[] args)
        {

            //获取连接
            var connection = UtilsRabbitmq.ConnectionUtils.GetConnection();
            //获取通道
            var channel = connection.CreateModel();
            //声明交换机
            channel.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Headers);
            var msg = "hello header";

            //发送消息
            var properties = channel.CreateBasicProperties();
            properties.Headers = new Dictionary<string, Object>
                {
                     {"info_type","sms"},
                     {"error_type","email"}
                };

            for (int i = 0; i < 10000000; i++)
            {
                channel.BasicPublish(EXCHANGE_NAME, "", false, properties, Encoding.UTF8.GetBytes(msg+i));
                Console.WriteLine("--send msg:" + msg+i);
                System.Threading.Thread.Sleep(3000);
            }

            Console.ReadKey();

            //channel.Close();
            //connection.Close();
        }
    }
}
