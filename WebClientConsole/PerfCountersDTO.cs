using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClientConsole
{
    public class PerfCounters
    {
        public string TimeStamp { get; set; }
        public double Cpu_Usage { get; set; }
        public double Ram_Usage { get; set; }
    }
    public class ServerTrack
    {
        public string Server { get; set; }
        public List<PerfCounters> Counters { get; set; }
    }
}
