using TOP_Network.Attributes;

namespace TOP_Packets
{
    public class Effect
    {
        public byte Attribute { get; set; }

        public ulong Value() => LongValue != 0 ? LongValue : ShortValue;

        [If("Attribute", 15)]
        [If("Attribute", 16)]
        [If("Attribute", 17)]
        [EndIf]
        public ulong LongValue { get; set; }

        public uint ShortValue { get; set; }
    }
}
