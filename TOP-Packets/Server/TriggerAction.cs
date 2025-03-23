using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOP_Packets.Server
{
    public class TriggerAction
    {
        public byte Type { get; set; }
        public short ID { get; set; }
        public short Number { get; set; }
        public short Count { get; set; }
    }
}
