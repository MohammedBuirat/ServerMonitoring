using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using Microsoft.Extensions.Configuration;


namespace ConsumerService
{
    public class ConsumeMessages
    {
        private readonly string _hostName;
        private readonly string _queueName;
        private readonly IConfiguration _configuration;

        public ConsumeMessages(IConfiguration configuration, string hostName, string queueName)
        {
            _hostName = hostName;
            _queueName = queueName;
            _configuration = configuration;
        }

        public async Task StartConsumingAsync()
        {
            var factory = new ConnectionFactory() { HostName = _hostName };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"Received Message: '{message}'");
                };

                channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

                Console.WriteLine($"Consumer started for queue: {_queueName} on host: {_hostName}. Press [Enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
