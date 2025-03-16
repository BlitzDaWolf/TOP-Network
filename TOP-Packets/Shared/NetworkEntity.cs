using System.ComponentModel;
using TOP_Network.Attributes;
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
        public byte SyncType { get; set; }
        public short TypeID { get; set; }

        public bool IsBoat { get; set; }

        [If("IsBoat", true)]
        [EndIf]
        public ushort Boat { get; set; }
        public short HairID { get; set; }

        [ArrayLength(10)]
        public NetworkItem[] Items { get; set; } = new NetworkItem[10];
    }

    public class SyncItem
    {
        public short Endure { get; set; }
        public short Energy { get; set; }
        public bool Valid { get; set; }
    }

    public class CreateItem
    {
        public short Number { get; set; }
        public short Endure1 { get; set; }
        public short Endure2 { get; set; }
        public short Energy1 { get; set; }
        public short Energy2 { get; set; }
        public byte Valid1 { get; set; }
        public bool Valid2 { get; set; }
    }

    public class NetworkItem
    {
        [ValidRecord(typeof(ItemTable))]
        public short ItemID { get; set; }

        public uint DatabaseID { get; set; }

        [If("SyncType", 1)]
        public SyncItem Sync { get; set; }

        [NotIf("SyncType", 1)]
        public CreateItem CreateItem { get; set; }

        public byte IsForge { get; set; }

        // [StopIf(false)]
        public bool Test { get; set; }

        [If("IsForge", true)]
        public int Forge { get; set; }
        [If("IsForge", true)]
        public int InstID { get; set; }

        [If("IsForge", true)]
        public bool V { get; set; }
    }
}
