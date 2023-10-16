using ConsumerService;
using ConsumerService.MessageQueues;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace ServerStatisticsCollectionService.MessageQueues
{
    public class RabbitMQMessageQueue : IMessageQueue
    {
        private readonly ConnectionFactory _factory;
        private readonly string _queueName;

        public RabbitMQMessageQueue(GetEnvironmentVariable envVariable)
        {
            string hostName = envVariable.GetConfigValue("RabbitMessageQueueHostName");
            _queueName = envVariable.GetConfigValue("RabbitMessageQueueQueueName");
            string password = envVariable.GetConfigValue("RabbitMessageQueuePassword") ?? "guest";
            string userName = envVariable.GetConfigValue("guest") ?? "guest";
            int port = int.Parse(envVariable.GetConfigValue("RabbitMessageQueuePort"));
            _factory = new ConnectionFactory()
            {
                HostName = hostName,
            };
        }

        public async Task<string> ConsumeAsync()
        {
            var tcs = new TaskCompletionSource<string>();

            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var consumedMessage = Encoding.UTF8.GetString(body);
                    tcs.SetResult(consumedMessage);
                };

                await Task.Run(() => channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer));
            }

            return await tcs.Task;
        }

    }
}

