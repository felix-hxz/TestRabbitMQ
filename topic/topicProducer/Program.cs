using System;
using RabbitMQ.Client;
using System.Text;

namespace topicProducer
{
    class Program
    {
        private const string EXCHANGE_NAME = "test_exchange_topic";
        static void Main(string[] args)
        {
            //获取连接
            var connection = UtilsRabbitmq.ConnectionUtils.GetConnection();
            //获取通道
            var channel = connection.CreateModel();
            //声明交换机
            channel.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Topic);

            var msg = "hello 商品";
            //发送消息
            channel.BasicPublish(EXCHANGE_NAME, "good.create", false, null, Encoding.UTF8.GetBytes(msg));

            Console.WriteLine("--send msg:" + msg);

            channel.Close();
            connection.Close();
        }
    }
}
