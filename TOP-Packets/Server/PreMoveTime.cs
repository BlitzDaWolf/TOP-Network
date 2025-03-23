using TOP_Network.Attributes;

namespace TOP_Packets.Server
{
    public class PreMoveTime
    {
        [SmallEndean]
        public long MoveTime { get; set; }
    }
}
