using ConsumerService.Entities;
using ConsumerService.Repositories;
using Microsoft.Extensions.Configuration;

namespace ConsumerService
{
    public class AnomalyDetection
    {
        private readonly IServerStatisticsRepository _serverStatisticsRepository;
        private readonly double memoryUsageAnomalyThreshold;
        private readonly double cpuUsageAnomalyThreshold;
        private readonly double memoryUsageThreshold;
        private readonly double cpuUsageThreshold;
        private readonly GetEnvironmentVariable _getEnvironmentVariable;

        public AnomalyDetection(IConfiguration configuration, IServerStatisticsRepository serverStatisticsRepository)
        {
            _getEnvironmentVariable = new GetEnvironmentVariable(configuration);
            _serverStatisticsRepository = serverStatisticsRepository;
            memoryUsageAnomalyThreshold = double.Parse(_getEnvironmentVariable.GetConfigValue("MemoryUsageAnomalyThresholdPercentage"));
            cpuUsageAnomalyThreshold = double.Parse(_getEnvironmentVariable.GetConfigValue("CpuUsageAnomalyThresholdPercentage"));
            memoryUsageThreshold = double.Parse(_getEnvironmentVariable.GetConfigValue("MemoryUsageThresholdPercentage"));
            cpuUsageThreshold = double.Parse(_getEnvironmentVariable.GetConfigValue("CpuUsageThresholdPercentage"));
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
