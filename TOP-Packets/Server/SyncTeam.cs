using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOP_Network.Attributes;
using TOP_Packets.Shared;
using static TOP_Packets.Shared.NetworkLook;

namespace TOP_Packets.Server
{
    public class SyncTeam
    {
        public class TeamLook
        {
            public uint ID { get; set; }
        }

        public uint EntityID { get; set; }
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int SP { get; set; }
        public int MaxSP { get; set; }
        public int Level { get; set; }



        public byte SyncType { get; set; }
        public short TypeID { get; set; }

        public bool IsBoat { get; set; }
        public short HairID { get; set; }

        [ArrayLength(10)]
        public TeamLook[] Look { get; set; }
    }
}
