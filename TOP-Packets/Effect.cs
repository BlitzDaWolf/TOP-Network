using TOP_Network.Attributes;

namespace TOP_Packets
{
    public class Effect
    {
        public byte Attribute { get; set; }

        public ulong Value() => LongValue != 0 ? LongValue : ShortValue;

        [If("Attribute", (byte)15)]
        [If("Attribute", (byte)16)]
        [If("Attribute", (byte)17)]
        [EndIf]
        [SmallEndean]
        public ulong LongValue { get; set; }

        public uint ShortValue { get; set; }
    }
}
