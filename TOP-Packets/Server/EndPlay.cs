using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOP_Network.Attributes;

namespace TOP_Packets.Server
{
    public class EndPlay
    {
        public short EnterError { get; set; }
        [NotIf("EnterError", (short)0)]
        [EndIf]
        public string Reason { get; set; }

        public LoginAccount[] Accounts { get; set; }
    }
}
