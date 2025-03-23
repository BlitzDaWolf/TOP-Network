using TOP_Network.Attributes;
using TOP_Packets.Shared;
using TOP_Records.Tables;

namespace TOP_Packets.Server
{
    public class SkillBag
    {
        public class Skill
        {
            public short ID { get; set; }
            public byte State{ get; set; }
            public byte Level { get; set; }
            public short UseSP { get; set; }
            public short UseEndure { get; set; }
            public short UseEnergy { get; set; }
            public long ResumeTime { get; set; }
            public long Skip1 { get; set; }

            [BreakIf(0)]
            public short Range { get; set; }

            [ArrayLength(3)]
            public short[] Ranges { get; set; }
        }

        public short SkillId { get; set; }
        public byte Type { get; set; }
        [ArraySize(typeof(short))]
        public Skill[] Amount { get; set; }
    }

    public class CharacterAttributes
    {
        public byte Type { get; set; }
        //public long Test { get; set; }
        //[ArrayLength(0x4A)]
        //public Effect[] Effects { get; set; }
        [ArraySize(typeof(short))]
        //[SmallEndean]
        public Effect[] Effects { get; set; }
    }

    public class KitBag
    {
        public class ItemAttribute
        {
            public short A1 { get; set; }
            public short A2 { get; set; }
        }

        public class KitBagItem
        {
            [ValidRecord(typeof(ItemTable))]
            // [SmallEndean]
            public ushort Item { get; set; } // 0x04
            public uint A { get; set; } // 0x08

            public short A1 { get; set; } // 0x0A
            public short A2 { get; set; } // 0x0C
            public short A3 { get; set; } // 0x0E
            public short A4 { get; set; } // 0x10
            public short A5 { get; set; } // 0x12

            public byte B1 { get; set; } // 0x13
            public bool B2 { get; set; } // 0x14

            [If("record.Type", (short)43)]
            public int R1 { get; set; }

            public int S1 { get; set; } // 0x18
            public int S2 { get; set; } // 0x1C

            public bool Valid { get; set; } // 0x1D
            [ArrayLength(5)]
            [If("Valid", true)]
            public ItemAttribute[] T1 { get; set; }
        }

        public byte Type { get; set; }
        [If("Type", (byte)0)]
        public short BagNumber { get; set; }

        [WhileNot(48, (short)-1, typeof(short))]
        public KitBagItem[] Items { get; set; }
    }

    public class ShortCut
    {
        public byte Type { get; set; }
        public short GridID { get; set; }
    }

    public class EnterMap
    {
        public short Enter { get; set; }
        [NotIf("Enter", (short)0)]
        public string Reason { get; set; }

        public bool AutoLock { get; set; }
        public bool KitBagLock { get; set; }

        public byte EnterType { get; set; }
        public bool IsNewCharacter { get; set; }
        public string MapName { get; set; }
        public bool CanTeam { get; set; }

        public byte A1 { get; set; }
        public uint A2 { get; set; }

        public NetworkEntity Entity { get; set; }

        public SkillBag SkillBag { get; set; }

        public SkillState SkillState { get; set; }

        public byte A4 { get; set; }
        [If("A4", (byte)250)]
        public short A5 { get; set; }
        [If("A4", (byte)250)]
        public int A6 { get; set; }

        public CharacterAttributes Attributes { get; set; }

        public KitBag KitBag { get; set; }

        [ArrayLength(0x24)]
        public ShortCut[] Shortcuts { get; set; }

        [ArraySize(typeof(short))]
        public NetworkBoat[] Boats { get; set; }
        [If("A4", (byte)250)]
        public byte Unkown { get; set; }
        public uint MainCharacterID { get; set; }
    }

    public class NetworkBoat
    {
        public NetworkEntity Boat { get; set; }
        public CharacterAttributes Attributes { get; set; }
        public KitBag KitBag { get; set; }
        public SkillState SkillState { get; set; }
    }
}
