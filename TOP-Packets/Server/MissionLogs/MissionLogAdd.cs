using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOP_Packets.Server.MissionLogs
{
    public class MissionLogAdd
    {
        public short ID { get; set; }
        public byte State { get; set; }
    }

    public class MissionLogClear
    {
        public short ID { get; set; }
    }
}
