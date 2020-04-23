using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using UtilsRabbitmq;


namespace confirmProducer
{
    class Program
    {   

        private const string EXCHANGE_NAME = "test_confirm_exchange";
        static void Main(string[] args)
        {   

            //获取连接
            var connection = ConnectionUtils.GetConnection();
            //获取通道
            var channnel = connection.CreateModel();
            //声明交换机
            channnel.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Direct, true, false, null);
            //将通道设置成confirm模式
            channnel.ConfirmSelect();
            //异步处理 Publisher Confirms
            var outstandingConfirms = new ConcurrentDictionary<ulong, string>();

            //success ack
            channnel.BasicAcks += (sender, ea) =>
            {
                outstandingConfirms.TryGetValue(ea.DeliveryTag, out string body);

                Console.WriteLine($"Message with body {body} has been ack-ed. " +
                    $"Sequence number: {ea.DeliveryTag}," +
                    $" multiple: {ea.Multiple}");

                //处理成功
                if (ea.Multiple)
                {
                    var confirmed = outstandingConfirms.Where(k => k.Key <= ea.DeliveryTag);
                    foreach (var entry in confirmed)
                    {
                        outstandingConfirms.TryRemove(entry.Key, out _);
                    }
                }
                else 
                {
                    outstandingConfirms.TryRemove(ea.DeliveryTag,out _);
                }
            };

            //error nack
            channnel.BasicNacks += (sender, ea) =>
            {
                //处理失败
                outstandingConfirms.TryGetValue(ea.DeliveryTag, out string body);
                Console.WriteLine($"Message with body {body} has been nack-ed. Sequence number: {ea.DeliveryTag}, multiple: {ea.Multiple}");
                if (ea.Multiple)
                {
                    var confirmed = outstandingConfirms.Where(k => k.Key <= ea.DeliveryTag);
                    foreach (var entry in confirmed)
                    {
                        outstandingConfirms.TryRemove(entry.Key, out _);
                    }
                }
                else
                {
                    outstandingConfirms.TryRemove(ea.DeliveryTag, out _);
                }
            };


            for (int i = 0; i < 2222222; i++)
            {   
                var msg = "第" + (i + 1) + "条消息";
                outstandingConfirms.TryAdd(channnel.NextPublishSeqNo,msg);
                //消息持久化
                var basicProperties = channnel.CreateBasicProperties();
                basicProperties.DeliveryMode = 2;
                channnel.BasicPublish(EXCHANGE_NAME, "info", basicProperties, Encoding.UTF8.GetBytes(msg));
    
                Console.WriteLine($"send msg:{msg}");

                System.Threading.Thread.Sleep(3000);
            }
            Console.ReadKey();
        }
    }
}
