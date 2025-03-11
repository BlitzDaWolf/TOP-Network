using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class LoginPacket
    {
        public string Bill { get; set; }// 
        public string Username { get; set; }// 
        public string Password { get; set; }//
        public string A1 { get; set; }//
        public string A2 { get; set; }//
        public string A3 { get; set; }//
        public string IP { get; set; }//
        public short Version { get; set; }// = 911,
        public short Version2 { get; set; }// = 171
    }
}
