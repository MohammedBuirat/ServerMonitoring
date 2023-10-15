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
    public class Parsers
    {
        public string ServerStatisticsToJson(ServerStatistics serverStatistics, string serverIdentifier)
        {
            string serverStatisticsJson = JsonConvert.SerializeObject(serverStatistics);
            var jsonData = new JObject();
            jsonData[serverIdentifier] = JToken.Parse(serverStatisticsJson);
            string jsonString = jsonData.ToString();
            return jsonString;
        }
    }
}
