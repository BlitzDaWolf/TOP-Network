using TOP_Packets.Shared;

namespace TOP_Packets.Server
{
    public class CharacterBeginSee
    {
        public byte SeeType { get; set; }
        public NetworkEntity Entity { get; set; }
    }
}
