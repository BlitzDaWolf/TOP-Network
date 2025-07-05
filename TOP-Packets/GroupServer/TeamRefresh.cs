using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOP_Packets.GroupServer
{
    public class TeamMember
    {
        public uint ID { get; set; }
        public string Name { get; set; }
        public string Motto { get; set; }
        public short Icon { get; set; }
    }

    public class TeamRefresh
    {
        public byte Kind { get; set; }
        // public byte Cound { get; set; }
        public TeamMember[] Members { get; set; }
    }
}
