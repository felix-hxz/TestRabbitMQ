using System;
using System.Text;
using UtilsRabbitmq;

namespace TestRabbitmqProducer
{   
    class Program
    {    

        private const string QUEUE_NAME = "test_simple_queue"; 
        static void Main(string[] args)
        {    
            //定义连接
            var connection = ConnectionUtils.GetConnection();
            //从连接中获取一个通道
            var channel = connection.CreateModel();
            //创建队列声明
            channel.QueueDeclare(QUEUE_NAME, false, false, false, null);

            for (int i = 0; i < 100000000; i++)
            {
                var msg = "hello simple" + i;

                channel.BasicPublish("", QUEUE_NAME, false, null, Encoding.UTF8.GetBytes(msg));

                Console.WriteLine("--send msg:" + msg);
                System.Threading.Thread.Sleep(3000);
            }

            Console.ReadKey();
            
            //关闭channel
            //channel.Close();
            //connection.Close();
        }
    }
}
