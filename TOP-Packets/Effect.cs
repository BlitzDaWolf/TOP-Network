using TOP_Network.Attributes;
using TOP_Network.Enum;

namespace TOP_Packets
{
    public class Effect
    {
        [ReadType(typeof(byte))]
        public EffectAttributes Attribute { get; set; }

        public ulong Value() => LongValue != 0 ? LongValue : ShortValue;

        [If("Attribute", (byte)15)]
        [If("Attribute", (byte)16)]
        [If("Attribute", (byte)17)]
        [EndIf]
        [SmallEndean]
        public ulong LongValue { get; set; }

        public uint ShortValue { get; set; }

        public override string ToString() => $"{(EffectAttributes)Attribute} {Value()}";
    }
}
