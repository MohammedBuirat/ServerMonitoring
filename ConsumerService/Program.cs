using ConsumerService.Repositories;
using Microsoft.Extensions.Configuration;
using ConsumerService.Entities;
using ConsumerService.MessageQueues;
using ServerStatisticsCollectionService.MessageQueues;

namespace ConsumerService
{
    public class Program
    {
        static async Task Main(string[] args)
        { 
            IConfiguration configuration = GetConfiguration();
            var getEnviornmentVariables = new GetEnvironmentVariable(configuration);
            IServerStatisticsRepository serverStatisticsRepository = new MongoServerStatisticsRepository(getEnviornmentVariables);
            AnomalyDetection anomalyDetection = new AnomalyDetection(getEnviornmentVariables, serverStatisticsRepository);
            var messageConsumer = new ConsumeMessages(anomalyDetection, serverStatisticsRepository);
            IMessageQueue messageQueue = new RabbitMQMessageQueue(getEnviornmentVariables, messageConsumer);
            messageQueue.ConsumeAsync();
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
