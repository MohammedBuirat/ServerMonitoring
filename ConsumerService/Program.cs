using Microsoft.Extensions.Configuration;
namespace ConsumerService
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            IConfiguration configuration = GetConfiguration();
            string hostName = "localhost";
            string queueName = "StatMessages";
            var messageConsumer = new ConsumeMessages(configuration, hostName, queueName);
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
