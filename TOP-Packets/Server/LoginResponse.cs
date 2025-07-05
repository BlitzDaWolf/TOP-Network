using System.Reflection;
using TOP_Network.Attributes;
using TOP_Network.Packets;

namespace TOP_Packets.Server
{
    public class LoginAccount
    {
        public class Look
        {
            [SmallEndean]
            public short Version { get; set; }
            [SmallEndean]
            public short Type { get; set; }

            [ArrayLength(10)]
            public Item[] Links { get; set; }
            [SmallEndean]
            public short HairID { get; set; }
        }

        public class Item
        {
            [SmallEndean]
            public int DBID { get; set; }
            [ArrayLength(2)]
            [SmallEndean]
            public long[] DBParam { get; set; }

            [SmallEndean]
            public short ID { get; set; }
            [SmallEndean]
            public short Number { get; set; }

            [ArrayLength(2)]
            [SmallEndean]
            public short[] Endure { get; set; }
            [ArrayLength(2)]
            [SmallEndean]
            public short[] Energy { get; set; }

            [SmallEndean]
            public byte ForgeLevel { get; set; }

            [ArrayLength(5)]
            [SmallEndean]
            public int[] Attr { get; set; }
            [ArrayLength(5)]
            [SmallEndean]
            public short[] Attr2 { get; set; }


            // Attr
            [ArrayLength(58)]
            [SmallEndean]
            public short[] Attributes { get; set; }
            [SmallEndean]
            public bool AttrInit { get; set; }
            
            public bool Valid { get; set; }
            public bool Change { get; set; }
        }

        [BreakIf(false)]
        public bool Valid { get; set; }

        public string Name { get; set; }
        public string Job { get; set; }
        public short Level { get; set; }

        public byte[] look { get; set; }
    }

    public class LoginResponse
    {
        public short EnterError { get; set; }
        [NotIf("EnterError", (short)0)]
        [EndIf]
        public string Reason { get; set; } = "";

        public byte[] ChatKey { get; set; }

        public LoginAccount[] Accounts { get; set; }
        
        [SmallEndean]
        public int Password { get; set; }
        public int CommunicationEncryption { get; set; }
        public int Flag { get; set; }
    }
}
