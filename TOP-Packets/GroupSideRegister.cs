using TOP_Network.Converter;
using TOP_Network.Enum;
using TOP_Packets.GroupServer;

namespace TOP_Packets
{
    public static class GroupSideRegister
    {
        public static void Register()
        {

            PacketToClass.AddType<TeamInvite>(Commands.CMD_PC_TEAM_INVITE);
            PacketToClass.AddType<Ping>(Commands.CMD_PC_PING);

            PacketToClass.AddType<GroupSay>(Commands.CMD_PC_SAY2TRADE);
            PacketToClass.AddType<GroupSay>(Commands.CMD_PC_SAY2ALL);
            PacketToClass.AddType<Refresh>(Commands.CMD_PC_MASTER_REFRESH);

            PacketToClass.AddType<SessionCreate>(Commands.CMD_PC_SESS_CREATE);
            PacketToClass.AddType<SessionLeave>(Commands.CMD_PC_SESS_LEAVE);
            PacketToClass.AddType<TeamRefresh>(Commands.CMD_PC_TEAM_REFRESH);
            PacketToClass.AddType<Refresh>(Commands.CMD_PC_FRND_REFRESH);
            PacketToClass.AddType<SessionSey>(Commands.CMD_PC_SESS_SAY);
        }
    }
}
