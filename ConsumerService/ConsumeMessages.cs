using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using Microsoft.Extensions.Configuration;
using ConsumerService.Entities;
using ConsumerService.Parser;
using ConsumerService.Repositories;

namespace ConsumerService
{
    public class ConsumeMessages
    {
        private readonly string _hostName;
        private readonly string _queueName;
        private readonly IConfiguration _configuration;
        private readonly Parsers _parser;
        private readonly IServerStatisticsRepository _serverStatisticsRepository;
        private readonly AnomalyDetection _anomalyDetection;

        public ConsumeMessages(AnomalyDetection anomalyDetection, IServerStatisticsRepository serverStatisticsRepository, IConfiguration configuration, string hostName, string queueName)
        {
            _serverStatisticsRepository = serverStatisticsRepository;
            _hostName = hostName;
            _queueName = queueName;
            _configuration = configuration;
            _parser = new Parsers();
            _anomalyDetection = anomalyDetection;
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
                    ReadOnlyMemory<byte> body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body.Span);
                    var serverStat = _parser.ParseJsonStringToServerStat(message);
                    _anomalyDetection.DetectAnomaly(serverStat);
                    _serverStatisticsRepository.InsertServerStatistics(serverStat);
                    
                };


                channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
                Console.ReadLine();
            }
        }
    }
}
