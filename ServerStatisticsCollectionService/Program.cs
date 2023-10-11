using Microsoft.Extensions.Configuration;
using ServerStatisticsCollectionService.MessageQueues;
namespace ServerStatisticsCollectionService
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            IConfiguration configuration = GetConfiguration();
            string hostName = "localhost";
            string queueName = "StatMessages";
            IMessageQueue messageQueue = new RabbitMQMessageQueue(hostName, queueName);
            var statCollectionSertvice = new ServerStatCollecting(configuration, messageQueue);
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