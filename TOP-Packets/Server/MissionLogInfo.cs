using System.ComponentModel;

namespace TOP_Packets.Server
{
    public class MissionLogInfo
    {
        public short MissionId { get; set; }

        [Description("Name of the mission")]
        public string MissionName { get; set; }

        [Description("Mission requerments for completion")]
        public MissionNeed[] MissionNeed { get; set; }

        public byte PrizeSellType { get; set; }

        public MissionPrize[] MissionPrize { get; set; }

        [Description("Mission description")]
        public string Description { get; set; }
    }
}
