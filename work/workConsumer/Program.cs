using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace workConsumer
{    
    
    class Program
    {
        private const string QUEUE_NAME = "test_work_queue1";
        static void Main(string[] args)
        {
            //获取连接
            var connection = UtilsRabbitmq.ConnectionUtils.GetConnection();
            //声明通道
            var channel = connection.CreateModel();
            //声明队列
            //channel.QueueDeclare(QUEUE_NAME, true,false, false, null);
            //保证每次只发一个
            channel.BasicQos(prefetchSize:0,prefetchCount:1,global:false);
            //定义一个消费者
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (ch, ea) =>
            {
                var msg = Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine("[1] consumer  msg:" + msg);
                System.Threading.Thread.Sleep(1000);
                //开启手动应答 发送回执
                channel.BasicAck(ea.DeliveryTag,false);
            };

            //监听队列
            //autoAck=false 关闭自动应答
            channel.BasicConsume(QUEUE_NAME, false, consumer);
            Console.ReadKey();
        }
    }
}
