using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOP_Packets.Server
{
    public class MissionLogInfo
    {
        public short MissionId { get; set; }

        public string MissionName { get; set; }

        public MissionNeed[] MissionNeed { get; set; }

        public byte PrizeSellType { get; set; }

        public MissionPrize[] MissionPrize { get; set; }

        public string Description { get; set; }
    }
}
