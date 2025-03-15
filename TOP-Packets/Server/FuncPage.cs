using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOP_Packets.Server
{
    public class Functions
    {
        public string Option { get; set; }
    }

    public class MissionFunction
    {
        public string Option { get; set; }
        public byte State { get; set; }
    }

    public class FuncPage
    {
        public uint NpcID { get; set; }
        public byte Page { get; set; }
        public string Talk { get; set; }

        public Functions[] Options { get; set; }
        public MissionFunction[] Missions { get; set; }
    }
}
