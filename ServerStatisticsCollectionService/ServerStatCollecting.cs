using ServerStatisticsCollectionService.Entities;
using System.Management;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ServerStatisticsCollectionService.MessageQueues;
using System.Diagnostics;
using ServerStatisticsCollectionService.Parser;
using System.Runtime.InteropServices;

namespace ServerStatisticsCollectionService
{
    public class ServerStatCollecting
    {
        private readonly GetEnvironmentVariable _getEnvironmentVariable;
        private readonly IMessageQueue _messageQueue;
        private readonly Parsers _parser;
        public ServerStatCollecting(GetEnvironmentVariable getEnvironmentVariable, IMessageQueue iMessageQueue)
        {
            _messageQueue = iMessageQueue;
            _getEnvironmentVariable = getEnvironmentVariable;
            _parser = new Parsers();
        }

        public async Task StartCollectingStatisticsAsync()
        {

            int samplingIntervalSeconds = int.Parse(_getEnvironmentVariable.GetConfigValue("SamplingIntervalSeconds"));
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

        public double GetAvailableMemory()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                try
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
                catch (PlatformNotSupportedException)
                {
                    return 0.0;
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                try
                {
                    string memInfoFilePath = "/proc/meminfo";
                    string memInfo = File.ReadAllText(memInfoFilePath);

                    var availableMemoryLine = memInfo
                        .Split('\n')
                        .FirstOrDefault(line => line.StartsWith("MemAvailable:", StringComparison.OrdinalIgnoreCase));

                    if (availableMemoryLine != null)
                    {
                        var availableMemoryInKB = long.Parse(availableMemoryLine
                            .Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)[1]);

                        return availableMemoryInKB / 1024.0;
                    }

                    return 0.0;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to retrieve available memory information on Linux.", ex);
                }
            }
            else
            {
                throw new PlatformNotSupportedException("Unsupported platform.");
            }
        }

        public double GetCpuUsage()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using (PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total"))
                {
                    cpuCounter.NextValue();
                    System.Threading.Thread.Sleep(1000);
                    double cpuUsage = cpuCounter.NextValue();

                    return cpuUsage;
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                try
                {
                    string procStatFilePath = "/proc/stat";
                    string[] lines = File.ReadAllLines(procStatFilePath);

                    // Find the line that starts with "cpu"
                    string cpuLine = lines.FirstOrDefault(line => line.StartsWith("cpu "));

                    if (cpuLine != null)
                    {
                        string[] values = cpuLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        // Parse CPU usage values
                        ulong user = ulong.Parse(values[1]);
                        ulong nice = ulong.Parse(values[2]);
                        ulong system = ulong.Parse(values[3]);
                        ulong idle = ulong.Parse(values[4]);
                        ulong total = user + nice + system + idle;

                        // Calculate CPU usage percentage
                        double cpuUsage = ((double)(total - idle) / total) * 100.0;

                        return cpuUsage;
                    }

                    return 0.0;
                }
                catch (Exception ex)
                {
                    return 0.0;
                }
            }
            else
            {
                throw new PlatformNotSupportedException("Unsupported platform.");
            }
        }

        private void PublishMessage(ServerStatistics serverStatistics)
        {
            string serverIdentifier = _getEnvironmentVariable.GetConfigValue("ServerIdentifier");
            string jsonString = _parser.ServerStatisticsToJson(serverStatistics, serverIdentifier);
            _messageQueue.Publish(jsonString);
        }
    }
}
