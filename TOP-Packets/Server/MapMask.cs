using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOP_Network.Attributes;

namespace TOP_Packets.Server
{
    public class MapMask
    {
        public uint EntityID { get; set; }
        public bool Valid { get; set; }
        [If("Valid", true)]
        public byte[] Mask { get; set; }
    }
}
