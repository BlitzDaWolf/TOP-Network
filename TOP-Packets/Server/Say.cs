using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOP_Packets.Server
{
    public class Say
    {
        public uint PlayerID { get; set; }
        public string Content { get; set; }
    }
}
