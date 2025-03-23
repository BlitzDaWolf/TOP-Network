using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOP_Network.Attributes;

namespace TOP_Packets.GroupServer
{
    public class SessionDetails
    {
        public uint CharacterID { get; set; }
        public string Name { get; set; }
        public string Motto { get; set; }
        public short Icon { get; set; }
    }

    public class SessionCreate
    {
        public uint SessionID { get; set; }
        [ArrayLength(2)]
        public SessionDetails[] Details { get; set; }
        public short Ammount { get; set; }
    }

    public class SessionLeave
    {
        public uint SessionID { get; set; }
        public uint EntityID { get; set; }
    }
}
