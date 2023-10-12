using ConsumerService.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ConsumerService.Repositories
{
    public class MongoServerStatisticsRepository : IServerStatisticsRepository
    {
        private readonly IMongoCollection<ServerStatistics> _collection;
        private readonly IMongoCollection<ServerStat> _insertCollection;

        public MongoServerStatisticsRepository(IConfiguration configuration)
        {
            var mongoConfigSection = configuration.GetSection("MongoDBConfig");
            string connectionString = mongoConfigSection["ConnectionString"];
            string databaseName = mongoConfigSection["DatabaseName"];
            string collectionName = "ServerStatistics";

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<ServerStatistics>(collectionName);
            _insertCollection = database.GetCollection<ServerStat>(collectionName);
        }

        public void InsertServerStatistics(ServerStatistics serverStatistics)
        {
            try
            {
                ServerStat serverStat = new ServerStat();
                serverStat.AvailableMemory = serverStatistics.AvailableMemory;
                serverStat.CpuUsage = serverStatistics.CpuUsage;
                serverStat.MemoryUsage = serverStatistics.MemoryUsage;
                serverStat.ServerIdentifier = serverStatistics.ServerIdentifier;
                serverStat.Timestamp = serverStatistics.Timestamp;
                _insertCollection.InsertOne(serverStat);
                Console.WriteLine("Server statistics inserted into MongoDB.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting server statistics into MongoDB: {ex.Message}");
            }
        }

        public ServerStatistics GetLastInsertedServerStatistics()
        {
            try
            {
                var filterDefinition = Builders<ServerStatistics>.Filter.Empty;
                var sortDefinition = Builders<ServerStatistics>.Sort.Descending("_id");
                var result = _collection.Find(filterDefinition).Sort(sortDefinition).Limit(1).FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving last inserted server statistics from MongoDB: {ex.Message}");
                return null;
            }
        }

        public ServerStatistics GetById(string id)
        {
            try
            {
                var filter = Builders<ServerStatistics>.Filter.Eq("_id", ObjectId.Parse(id));
                return _collection.Find(filter).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving server statistics from MongoDB: {ex.Message}");
                return null;
            }
        }
    }
}
