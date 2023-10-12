using ConsumerService;
using Microsoft.Extensions.Configuration;
using ServerStatisticsCollectionService.MessageQueues;
using System;
using System.Threading.Tasks;

namespace ServerStatisticsCollectionService
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            IConfiguration configuration = GetConfiguration();
            var environmentVariableProvider = new GetEnvironmentVariable(configuration);
            string hostName = environmentVariableProvider.GetConfigValue("RabbitMQHostName") ?? "localhost";
            string queueName = environmentVariableProvider.GetConfigValue("RabbitMQQueueName") ?? "StatMessages";
            IMessageQueue messageQueue = new RabbitMQMessageQueue(hostName, queueName);
            var statCollectionService = new ServerStatCollecting(configuration, messageQueue, environmentVariableProvider);
            await statCollectionService.StartCollectingStatisticsAsync();
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
