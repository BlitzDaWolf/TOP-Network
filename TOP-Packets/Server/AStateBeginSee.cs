using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOP_Network.Attributes;

namespace TOP_Packets.Server
{
    public class State
    {
        public short Valid { get; set; }

        [NotIf("Valid", (short)0)]
        public byte D { get; set; }
        [NotIf("Valid", (short)0)]
        public long A { get; set; }
        [NotIf("Valid", (short)0)]
        public long B { get; set; }
        [NotIf("Valid", (short)0)]
        public uint ID { get; set; }
        [NotIf("Valid", (short)0)]
        public byte C { get; set; }
    }

    public class AStateBeginSee
    {
        public short AreaX{ get; set; }
        public short AreaY{ get; set; }
        [ArraySize(typeof(short))]
        public State[] States { get; set; }
    }
}
