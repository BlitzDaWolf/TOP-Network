using TOP_Network.Attributes;

namespace TOP_Packets.Server
{
    public class Emotion
    {
        public uint Character { get; set; }
        public short Emote { get; set; }

        [If("Emote", (short)-1)]
        [SmallEndean]
        public long Time { get; set; }
    }
}
