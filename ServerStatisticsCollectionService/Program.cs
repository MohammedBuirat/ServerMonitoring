using Microsoft.Extensions.Configuration;
using ServerStatisticsCollectionService.MessageQueues;
namespace ServerStatisticsCollectionService
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            IConfiguration configuration = GetConfiguration();
            GetEnvironmentVariable getEnvironmentVariable = new GetEnvironmentVariable(configuration);
            IMessageQueue messageQueue = new RabbitMQMessageQueue(getEnvironmentVariable);
            var statCollectionSertvice = new ServerStatCollecting(getEnvironmentVariable, messageQueue);
            await statCollectionSertvice.StartCollectingStatisticsAsync();
        }

        static IConfiguration GetConfiguration()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            return configuration;
        }
    }
}