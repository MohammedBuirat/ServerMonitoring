using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerService.Entities
{
    public class ServerStatistics
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public String ServerIdentifier { get; set; }
        public double MemoryUsage { get; set; } // in MB
        public double AvailableMemory { get; set; } // in MB
        public double CpuUsage { get; set; }
        public DateTime Timestamp { get; set; }

        public override string ToString()
        {
            return $"Identifier: {ServerIdentifier}     Memory Usage: {MemoryUsage}     Time: {Timestamp}";
        }
    }
}
