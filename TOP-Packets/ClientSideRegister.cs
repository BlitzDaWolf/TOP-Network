using TOP_Network.Converter;
using TOP_Network.Enum;
using TOP_Packets.Client;
using TOP_Packets.GroupServer;

namespace TOP_Packets
{
    public static class ClientSideRegister
    {
        public static void Register()
        {
            PacketToClass.AddType<BeginAction>(Commands.CMD_CM_BEGINACTION);
            PacketToClass.AddType<BeginPlay>(Commands.CMD_CM_BGNPLAY);
            PacketToClass.AddType<ClientPing>(Commands.CMD_CM_CHECK_PING);
            PacketToClass.AddType<DieReturn>(Commands.CMD_CM_DIE_RETURN);
            PacketToClass.AddType<RequestMapMask>(Commands.CMD_CM_MAP_MASK);
            PacketToClass.AddType<RequestTalk>(Commands.CMD_CM_REQUESTTALK);
            PacketToClass.AddType<RequestTrade>(Commands.CMD_CM_REQUESTTRADE);
            PacketToClass.AddType<ClientSay>(Commands.CMD_CM_SAY);
            PacketToClass.AddType<Ping>(Commands.CMD_CP_PING);
            PacketToClass.AddType<SessionSay>(Commands.CMD_CP_SESS_SAY);
        }
    }
}
