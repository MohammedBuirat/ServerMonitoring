using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ConsumerService.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerService.Parser
{
    public class Parsers
    {
        public ServerStatistics ParseJsonStringToServerStat(string jsonString)
        {
            var serverData = JsonConvert.DeserializeObject<Dictionary<string, ServerStatistics>>(jsonString);
            var firstKvp = serverData.First();
            string serverIdentifier = firstKvp.Key;
            ServerStatistics serverStat = firstKvp.Value;
            serverStat.ServerIdentifier = serverIdentifier;
            return serverStat;
        }
    }
}
