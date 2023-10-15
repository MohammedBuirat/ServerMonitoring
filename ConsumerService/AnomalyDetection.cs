using ConsumerService.Entities;
using ConsumerService.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerService
{
    public class AnomalyDetection
    {
        private readonly IServerStatisticsRepository _serverStatisticsRepository;
        private readonly double memoryUsageAnomalyThreshold;
        private readonly double cpuUsageAnomalyThreshold;
        private readonly double memoryUsageThreshold;
        private readonly double cpuUsageThreshold;

        public AnomalyDetection(IConfiguration configuration, IServerStatisticsRepository serverStatisticsRepository)
        {
            _serverStatisticsRepository = serverStatisticsRepository;

            var anomalyDetectionConfigSection = configuration.GetSection("AnomalyDetectionConfig");
            memoryUsageAnomalyThreshold = double.Parse(anomalyDetectionConfigSection["MemoryUsageAnomalyThresholdPercentage"]);
            cpuUsageAnomalyThreshold = double.Parse(anomalyDetectionConfigSection["CpuUsageAnomalyThresholdPercentage"]);
            memoryUsageThreshold = double.Parse(anomalyDetectionConfigSection["MemoryUsageThresholdPercentage"]);
            cpuUsageThreshold = double.Parse(anomalyDetectionConfigSection["CpuUsageThresholdPercentage"]);

            var mongoConfigSection = configuration.GetSection("MongoDBConfig");
            string connectionString = mongoConfigSection["ConnectionString"];
            string databaseName = mongoConfigSection["DatabaseName"];
        }

        public void DetectAnomaly(ServerStatistics serverStatistics)
        {
            ServerStatistics previousServerStatistics = _serverStatisticsRepository.GetLastInsertedServerStatistics();
            bool memoryUsageAnomaly = serverStatistics.MemoryUsage > (previousServerStatistics.MemoryUsage * (1 + memoryUsageAnomalyThreshold));
            bool cpuUsageAnomaly = serverStatistics.CpuUsage > (previousServerStatistics.CpuUsage * (1 + cpuUsageAnomalyThreshold));

            double totalMemory = serverStatistics.MemoryUsage + serverStatistics.AvailableMemory;
            bool memoryHighUsageAlert = (serverStatistics.MemoryUsage / totalMemory) > memoryUsageThreshold;
            bool cpuHighUsageAlert = serverStatistics.CpuUsage > (100 * cpuUsageThreshold);

            if (memoryUsageAnomaly || cpuUsageAnomaly)
            {
                Console.WriteLine($"Anomaly Detected - Memory Usage Anomaly: {memoryUsageAnomaly}, CPU Usage Anomaly: {cpuUsageAnomaly}");
            }

            if (memoryHighUsageAlert || cpuHighUsageAlert)
            {
                Console.WriteLine($"High Usage Alert - Memory High Usage: {memoryHighUsageAlert}, CPU High Usage: {cpuHighUsageAlert}");
            }

            _serverStatisticsRepository.InsertServerStatistics(serverStatistics);
        }
    }
}
