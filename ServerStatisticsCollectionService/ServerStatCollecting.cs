using ServerStatisticsCollectionService.Entities;
using Microsoft.Extensions.Configuration;
using System.Management;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ServerStatisticsCollectionService.MessageQueues;
using System.Diagnostics;
using ServerStatisticsCollectionService.Parser;
using ConsumerService;

namespace ServerStatisticsCollectionService
{
    public class ServerStatCollecting
    {
        private readonly IConfiguration _configuration;
        private readonly IMessageQueue _messageQueue;
        private readonly Parsers _parser;
        private readonly GetEnvironmentVariable _environmentVariableProvider;

        public ServerStatCollecting(IConfiguration configuration, IMessageQueue messageQueue, GetEnvironmentVariable environmentVariableProvider)
        {
            _configuration = configuration;
            _messageQueue = messageQueue;
            _parser = new Parsers();
            _environmentVariableProvider = environmentVariableProvider;
        }

        public async Task StartCollectingStatisticsAsync()
        {
            int samplingIntervalSeconds = int.Parse(_environmentVariableProvider.GetConfigValue("SamplingIntervalSeconds") ?? "60");
            while (true)
            {
                var serverStatistics = new ServerStatistics
                {
                    MemoryUsage = GetMemoryUsage(),
                    AvailableMemory = GetAvailableMemory(),
                    CpuUsage = GetCpuUsage(),
                    Timestamp = DateTime.UtcNow
                };
                PublishMessage(serverStatistics);
                await Task.Delay(samplingIntervalSeconds * 1000);
            }
        }


        private double GetMemoryUsage()
        {
            return System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / (1024.0 * 1024.0);
        }

        private double GetAvailableMemory()
        {
            ObjectQuery wql = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(wql);
            ManagementObjectCollection results = searcher.Get();

            foreach (ManagementObject result in results)
            {
                ulong totalVisibleMemoryKB = Convert.ToUInt64(result["TotalVisibleMemorySize"]);
                ulong freePhysicalMemoryKB = Convert.ToUInt64(result["FreePhysicalMemory"]);

                double totalAvailableMemoryMB = (double)(totalVisibleMemoryKB - freePhysicalMemoryKB) / 1024.0;
                return totalAvailableMemoryMB;
            }
            return 0.0;
        }

        private double GetCpuUsage()
        {
            using (PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total"))
            {
                cpuCounter.NextValue();
                System.Threading.Thread.Sleep(1000);
                double cpuUsage = cpuCounter.NextValue();

                return cpuUsage;
            }
        }

        private void PublishMessage(ServerStatistics serverStatistics)
        {
            string serverIdentifier = _environmentVariableProvider.GetConfigValue("ServerIdentifier");
            string jsonString = _parser.ServerStatisticsToJson(serverStatistics, serverIdentifier);
            _messageQueue.Publish(jsonString);
        }
    }
}

