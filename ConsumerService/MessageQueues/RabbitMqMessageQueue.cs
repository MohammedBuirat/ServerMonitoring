using ConsumerService.MessageQueues;
using ConsumerService;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ServerStatisticsCollectionService.MessageQueues
{
    public class RabbitMQMessageQueue : IMessageQueue
    {
        private readonly ConnectionFactory _factory;
        private readonly string _queueName;
        private readonly ConsumeMessages _consumeMessages;

        public RabbitMQMessageQueue(GetEnvironmentVariable envVariable, ConsumeMessages consumeMessages)
        {
            _consumeMessages = consumeMessages;
            string hostName = envVariable.GetConfigValue("RabbitMessageQueueConnection");
            _queueName = envVariable.GetConfigValue("RabbitMessageQueueQueueName");
            _factory = new ConnectionFactory()
            {
                HostName = hostName
            };
        }

        public async Task ConsumeAsync()
        {
            while (true)
            {
                using (var connection = _factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: _queueName,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += async (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        await _consumeMessages.StartConsumingAsync(message);
                    };

                    channel.BasicConsume(queue: _queueName,
                                         autoAck: true,
                                         consumer: consumer);
                    Console.ReadLine();
                }
            }
        }
    }
}
