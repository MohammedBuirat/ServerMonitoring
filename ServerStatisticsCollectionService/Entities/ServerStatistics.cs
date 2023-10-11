using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMonitoring.Entities
{
    public class ServerStatistics
    {
        public double MemoryUsage { get; set; } // in MB
        public double AvailableMemory { get; set; } // in MB
        public double CpuUsage { get; set; }
        public DateTime Timestamp { get; set; }

        public override string ToString()
        {
            return $"Memory usage: {MemoryUsage}     AvailableMemory: {AvailableMemory}   Cpu usage: {CpuUsage}    Time stamp: {Timestamp}";
        }
    }
}
