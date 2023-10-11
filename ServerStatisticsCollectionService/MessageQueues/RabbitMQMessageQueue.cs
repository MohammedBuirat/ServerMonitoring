using RabbitMQ.Client;
using System.Text;

namespace ServerStatisticsCollectionService.MessageQueues
{
    public class RabbitMQMessageQueue : IMessageQueue
    {
        private readonly ConnectionFactory _factory;
        private readonly string _queueName;

        public RabbitMQMessageQueue(string hostName, string queueName)
        {
            _factory = new ConnectionFactory() { HostName = hostName };
            _queueName = queueName;
        }

        public void Publish(string serverStatisticsMessage)
        {
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                var body = Encoding.UTF8.GetBytes(serverStatisticsMessage);
                channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
            }
        }
    }
}
