using ServerStatisticsCollectionService.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ServerStatisticsCollectionService.MessageQueues
{
    public interface IMessageQueue
    {
        public void Publish(string serverStatisticsMessage);
    }
}
