using System.ComponentModel;

namespace TOP_Packets.Server
{
    public class Net_mis
    {
        [Description("The ID of the mission. Mostly used on the server")]
        public short MissionId { get; set; }
        [Description("State of the mission (Incomplete, Failed, Completed)")]
        public bool State { get; set; }
    }

    public class MissionLog
    {
        [Description("List of all atcite missions")]
        public Net_mis[] Missions { get; set; }
    }
}
