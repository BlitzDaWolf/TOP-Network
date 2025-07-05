using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOP_Network.Attributes;

namespace TOP_Packets.Server
{
    public class Skill
    {
        /*public short SkillID { get; set; }
        public byte Level { get; set; }*/
        public short SkillID { get; set; }
        public byte Level { get; set; }
        public long A { get; set; }
        public long B { get; set; }
    }

    public class SyncSkillState
    {
        public uint CharacterID { get; set; }
        [ArraySize(typeof(short))]
        public Skill[] Skills { get; set; }
    }
}
