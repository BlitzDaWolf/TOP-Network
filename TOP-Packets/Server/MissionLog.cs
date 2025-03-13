namespace TOP_Packets.Server
{

    public struct Net_mis
    {
        public short MissionId { get; set; }
        public bool State { get; set; }
    }

    public class MissionLog
    {
        public Net_mis[] Missions { get; set; }
    }
}
