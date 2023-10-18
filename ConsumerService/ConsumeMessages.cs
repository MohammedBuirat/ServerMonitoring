using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using ConsumerService.Entities;
using ConsumerService.Parser;
using ConsumerService.Repositories;

namespace ConsumerService
{
    public class ConsumeMessages
    {
        private readonly Parsers _parser;
        private readonly IServerStatisticsRepository _serverStatisticsRepository;
        private readonly AnomalyDetection _anomalyDetection;

        public ConsumeMessages(AnomalyDetection anomalyDetection, IServerStatisticsRepository serverStatisticsRepository)
        {
            _serverStatisticsRepository = serverStatisticsRepository;
            _parser = new Parsers();
            _anomalyDetection = anomalyDetection;

        }

        public async Task StartConsumingAsync(string message)
        {
            var serverStat = _parser.ParseJsonStringToServerStat(message);
            _anomalyDetection.DetectAnomaly(serverStat);
            _serverStatisticsRepository.InsertServerStatistics(serverStat);
        }
    }
}

