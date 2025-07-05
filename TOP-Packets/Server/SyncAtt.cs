using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOP_Network.Attributes;

namespace TOP_Packets.Server
{
    public class SyncAtt
    {
        public uint ID { get; set; }
        public byte Type { get; set; }
        [ArraySize(typeof(short))]
        public Effect[] Attributes { get; set; }
    }
}
