using System;
using UtilsRabbitmq;
using RabbitMQ.Client;
using System.Text;

namespace workProducer
{
    class Program
    {
        /*                     |------------C1
        *                      |
        *    p-----------------|
        *                      |------------C2
        *                      
        */
        private const string QUEUE_NAME = "test_work_queue1";
        static void Main(string[] args)
        {
            //获取连接
            var connection = ConnectionUtils.GetConnection();
            //获取channel
            var channel = connection.CreateModel();
            //保证每次只发一个 在消费者没有ack之前
            channel.BasicQos(prefetchSize:0,prefetchCount:1,global:false);

            //声明队列    bool durable=true消息持久化
            channel.QueueDeclare(QUEUE_NAME, true, false, false, null);

            for (int i = 0; i < 50; i++)
            {
                string msg = "hello" + i;
                channel.BasicPublish("", QUEUE_NAME, false, null, Encoding.UTF8.GetBytes(msg));
                System.Threading.Thread.Sleep(i * 20);
                Console.WriteLine("--send msg:" + msg);
            }
            channel.Close();
            connection.Close();
        }
    }
}
