using Microsoft.VisualBasic;
using TOP_Network.Attributes;
using TOP_Records.Tables;

namespace TOP_Packets.Server
{
    public abstract class MissionCompletion;

    public class MissionCollect : MissionCompletion
    {
#if !DEBUG
        [ValidRecord(typeof(ItemTable))]
#endif
        public short ItemId { get; set; }
        public short Need { get; set; }
        public byte Param3 { get; set; }
    }
    public class MissionKill : MissionCompletion
    {
#if !DEBUG
        [ValidRecord(typeof(CharacterTable))]
#endif
        public short MonsterId { get; set; }
        public short Need { get; set; }
        public byte Param3 { get; set; }
    }

    public class MissionDescription : MissionCompletion
    {
        public string Description { get; set; }
    }

    public class MissionNeed
    {
        [Choose(0, typeof(MissionCollect))]
        [Choose(1, typeof(MissionKill))]
        [Choose(5, typeof(MissionDescription))]
        public MissionCompletion Completion { get; set; }
    }

    public class MissionPrize
    {
        public byte Char { get; set; }
        public short Param1 { get; set; }
        public short Param2 { get; set; }
    }

    public class MissionPage
    {
        public byte Type { get; set; }
        public uint NpcID { get; set; }

        public string MissionName { get; set; }

        public MissionNeed[] MissionNeed { get; set; }

        public byte PrizeSellType { get; set; }

        public MissionPrize[] MissionPrize { get; set; }

        public string Description { get; set; }
    }
}
