using ConsumerService.Repositories;
using Microsoft.Extensions.Configuration;
using ConsumerService.Entities;

namespace ConsumerService
{
    public class Program
    {
        static async Task Main(string[] args)
        { 
            IConfiguration configuration = GetConfiguration();
            string hostName = "localhost";
            string queueName = "StatMessages";
            IServerStatisticsRepository serverStatisticsRepository = new MongoServerStatisticsRepository(configuration);
            AnomalyDetection anomalyDetection = new AnomalyDetection(configuration, serverStatisticsRepository);
            var messageConsumer = new ConsumeMessages(anomalyDetection, serverStatisticsRepository, configuration, hostName, queueName);
            await messageConsumer.StartConsumingAsync();
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
