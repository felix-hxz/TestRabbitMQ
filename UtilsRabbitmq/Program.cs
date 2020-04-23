using RabbitMQ.Client;


namespace UtilsRabbitmq
{

    /// <summary>
    /// 管理连接connection
    /// </summary>
    public class ConnectionUtils 
    {   
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IConnection GetConnection()  
        {
            //创建连接工厂
            var factory = new ConnectionFactory
            {
                HostName = "120.77.144.4",
                Port = 5672,              //amqp
                VirtualHost = "dev",     //vhost
                UserName = "admin",
                Password = "1qa@WS3ed"
            };
            //创建连接
            return factory.CreateConnection();
        }
    }
    
}
