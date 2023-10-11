using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ServerStatisticsCollectionService.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerStatisticsCollectionService.Parser
{
    public class Parser
    {
        public string ServerStatisticsToJson(ServerStatistics serverStatistics)
        {
            var serverStatisticsConfigSection = _configuration.GetSection("ServerStatisticsConfig");
            string serverIdentifier = serverStatisticsConfigSection["ServerIdentifier"];
            string serverStatisticsJson = JsonConvert.SerializeObject(serverStatistics);
            var jsonData = new JObject();
            jsonData[serverIdentifier] = JToken.Parse(serverStatisticsJson);
            string jsonString = jsonData.ToString();
            return jsonString;
        }
    }
}
