namespace TOP_Packets.Server
{
    public class CharacterEndSee
    {
        public byte Type { get; set; }
        public uint EntityID { get; set; }
    }
    public class ItemEndSee
    {
        public uint EntityID { get; set; }
    }
    public class AsteEndSee
    {
        public uint EntityID { get; set; }
    }
}
