using ConsumerService.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerService.Repositories
{
    public interface IServerStatisticsRepository
    {
        public void InsertServerStatistics(ServerStatistics serverStatistics);

        public ServerStatistics GetLastInsertedServerStatistics();

        public ServerStatistics GetById(string id);
    }
}
