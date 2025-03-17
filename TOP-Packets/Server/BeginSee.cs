using TOP_Packets.Shared;

namespace TOP_Packets.Server
{
    public class CharacterBeginSee
    {
        public byte SeeType { get; set; }
        public NetworkEntity Entity { get; set; }
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
