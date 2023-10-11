using Microsoft.Extensions.Configuration;
namespace ConsumerService
{
    public class Program
    {
        static void Main(string[] args)
        {
            IConfiguration configuration = GetConfiguration();

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
