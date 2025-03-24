using System.ComponentModel;
using TOP_Network.Attributes;
using TOP_Packets.Server;
using TOP_Records.Tables;

namespace TOP_Packets.Shared
{
    public class Guild
    {
        public uint GuildID { get; set; }
        public string GuildName { get; set; }
        public string GuildMotto { get; set; }
    }

    public class NetworkEntity
    {
        public uint CharacterId { get; set; }
        public uint WoldID { get; set; }

        public uint ComID { get; set; }
        public string ComName { get; set; }
        public byte GMLevel { get; set; }

        public uint Handle { get; set; }
        public byte ControllType { get; set; }

        public string Name { get; set; }
        public string Motto { get; set; }

        public short Icon { get; set; }

        public byte Skip1 { get; set; }

        public Guild Guild { get; set; }

        public string StallName { get; set; }
        //public ushort Skip { get; set; }
        public int State { get; set; }

        #region Position
        public int X { get; set; }
        public int Y { get; set; }
        public uint Radius { get; set; }
        public short Angle { get; set; }
        #endregion

        public uint TeamLeader { get; set; }
        public byte SideID { get; set; }

        public Event Event { get; set; }

        public NetworkLook Look { get; set; }
        public byte PKControll { get; set; }
        [ArrayLength(4)]
        public Look[] AppendLook { get; set; }
    }

    public class Event
    {
        public uint EntityId { get; set; }
        public byte EntityType { get; set; }
        public short EventID { get; set; }
        public string EventName { get; set; }
    }

    public class NetworkLook
    {
        public class BoatLook
        {
            public short PosID { get; set; }
            public short BoatID { get; set; }
            public short Header { get; set; }
            public short Body { get; set; }
            public short Engine { get; set; }
            public short Cannon { get; set; }
            public short Equipment { get; set; }
        }

        public byte SyncType { get; set; }
        public short TypeID { get; set; }

        public bool IsBoat { get; set; }

        [If("IsBoat", true)]
        [EndIf]
        public BoatLook Boat { get; set; }
        public short HairID { get; set; }

        [ArrayLength(10)]
        public NetworkItem[] Items { get; set; } = new NetworkItem[10];
    }

    public class PKControll
    {
        public byte A { get; set; }
        public byte B { get; set; }
        public byte C { get; set; }
    }

    public abstract class SyncType;

    public class SyncItem : SyncType
    {
        public short Endure { get; set; }
        public short Energy { get; set; }
        public bool Valid { get; set; }
    }

    public class CreateItem : SyncType
    {
        public class Test
        {
            public short AA { get; set; }
            public short BB { get; set; }
        }

        public class itemAttribute
        {

            public int A { get; set; }
            public int B { get; set; }
            public byte Go { get; set; }

            [NotIf("Go", (byte)0)]
            [ArrayLength(5)]
            public Test[] Test { get; set; }
        }

        public short Number { get; set; }
        public short Endure1 { get; set; }
        public short Endure2 { get; set; }
        public short Energy1 { get; set; }
        public short Energy2 { get; set; }
        public byte TB { get; set; }
        public byte TA { get; set; }
        public bool Valid { get; set; }

        [If("Valid", true)]
        public itemAttribute ItemAtribute { get; set; }
    }

    public class NetworkItem
    {
        // [ValidRecord(typeof(ItemTable))]
        [BreakIf((short)0)]
        public short ItemID { get; set; }

        public uint DatabaseID { get; set; }

        [If("SyncType", (byte)1)]
        public SyncItem Sync { get; set; }

        [NotIf("SyncType", (byte)1)]
        public CreateItem CreateItem { get; set; }
    }
}
