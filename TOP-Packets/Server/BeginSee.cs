using TOP_Network.Attributes;
using TOP_Packets.Shared;

namespace TOP_Packets.Server
{
    public class Lean
    {
        public byte State { get; set; }
        public int Pose { get; set; }
        public int Angle { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
    }

    public class CharacterBeginSee
    {
        public byte SeeType { get; set; }
        public NetworkEntity Entity { get; set; }
        public short TestA { get; set; }
        public short TestB { get; set; }

        [If("TestB", (short)1)]
        public Lean Lean { get; set; }

        public CharacterAttributes Attributes { get; set; }
        [ArraySize(typeof(short))]
        public Skill[] Skills { get; set; }
    }

    public class ItemBeginSee
    {
        public uint WorldID { get; set; }
        public uint Handle { get; set; }
        public uint ItemID { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public short Angle { get; set; }
        public short Amount { get; set; }
        public byte ItemLook { get; set; }

        public uint FromEntity { get; set; }

        public Event EnterEvent { get; set; }
    }
}
