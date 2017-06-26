using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthMonitoring
{
    class Stats
    {

        // Miner stats
        public string version;
        public string total_hashrate;
        public string total_dual_hashrate;
        public int type;
        public List<string> hashrates;
        public List<string> dcr_hashrates;
        public List<string> temps;
        public List<string> fan_speeds;
        public List<string> power_usage;
        public Boolean online;
        public string uptime;

        // Exception
        public Exception ex;

        // Pool stats
        public int accepted;
        public int rejected;
        public int dual_accepted;
        public int dual_rejected;

    }
}
