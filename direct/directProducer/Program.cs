using System;
using RabbitMQ.Client;
using System.Text;

namespace directProducer
{
    class Program
    {
        private const string EXCHANGE_NAME = "test_exchange_direct";
        static void Main(string[] args)
        {
            //获取连接
            var connection = UtilsRabbitmq.ConnectionUtils.GetConnection();
            //获取通道
            var channel = connection.CreateModel();
            //声明交换机
            channel.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Direct);

            var msg = "hello direct";
            //发送消息
            channel.BasicPublish(EXCHANGE_NAME, "error", false, null, Encoding.UTF8.GetBytes(msg));

            Console.WriteLine("--send msg:" + msg);

            channel.Close();
            connection.Close();
        }
    }
}
