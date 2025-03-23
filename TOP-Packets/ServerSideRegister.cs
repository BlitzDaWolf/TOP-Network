using TOP_Network.Enum;
using TOP_Packets.Server.MissionLogs;
using TOP_Packets.Server;
using TOP_Network.Converter;

namespace TOP_Packets
{
    public static class ServerSideRegister
    {
        public static void Register()
        {
            PacketToClass.AddType<CharacterBeginSee>(Commands.CMD_MC_CHABEGINSEE);
            PacketToClass.AddType<EnterMap>(Commands.CMD_MC_ENTERMAP);
            PacketToClass.AddType<LoginResponse>(Commands.CMD_MC_LOGIN);
            PacketToClass.AddType<StallData>(Commands.CMD_MC_STALL_ALLDATA);
            PacketToClass.AddType<Say>(Commands.CMD_MC_SAY);
            PacketToClass.AddType<MissionLogClear>(Commands.CMD_MC_MISLOG_CLEAR);
            PacketToClass.AddType<MissionLogAdd>(Commands.CMD_MC_MISLOG_ADD);
            PacketToClass.AddType<SyncSkill>(Commands.CMD_MC_SYNSKILLBAG);
            PacketToClass.AddType<EndPlay>(Commands.CMD_MC_ENDPLAY);
            PacketToClass.AddType<SyncTeam>(Commands.CMD_MC_TEAM);
            PacketToClass.AddType<ItemBeginSee>(Commands.CMD_MC_ITEMBEGINSEE);
            PacketToClass.AddType<AsteEndSee>(Commands.CMD_MC_ASTATEENDSEE);
            PacketToClass.AddType<LeaderID>(Commands.CMD_MC_TLEADER_ID);
            PacketToClass.AddType<KitBagSync>(Commands.CMD_MC_KITBAGTEMP_SYNC);
            PacketToClass.AddType<TriggerAction>(Commands.CMD_MC_TRIGGER_ACTION);
            PacketToClass.AddType<MapMask>(Commands.CMD_MC_MAP_MASK);
            PacketToClass.AddType<Emotion>(Commands.CMD_MC_CHA_EMOTION);
            PacketToClass.AddType<CHAPSTR>(Commands.CMD_MC_CHAPSTR);
            PacketToClass.AddType<Notification>(Commands.CMD_MC_NOTIACTION);
            PacketToClass.AddType<FuncPage>(Commands.CMD_MC_FUNCPAGE);
            PacketToClass.AddType<MissionLog>(Commands.CMD_MC_MISLOG);
            PacketToClass.AddType<MissionPage>(Commands.CMD_MC_MISPAGE);
            PacketToClass.AddType<MissionLogInfo>(Commands.CMD_MC_MISLOGINFO);
            PacketToClass.AddType<NpcStateChange>(Commands.CMD_MC_NPCSTATECHG);
            PacketToClass.AddType<SystemInformation>(Commands.CMD_MC_SYSINFO);
            PacketToClass.AddType<SyncAtt>(Commands.CMD_MC_SYNATTR);
            PacketToClass.AddType<SyncSkillState>(Commands.CMD_MC_SYNASKILLSTATE);
            PacketToClass.AddType<AStateBeginSee>(Commands.CMD_MC_ASTATEBEGINSEE);
            PacketToClass.AddType<PreMoveTime>(Commands.CMD_MC_PREMOVE_TIME);
            PacketToClass.AddType<AppendLook>(Commands.CMD_MC_APPEND_LOOK);

            PacketToClass.AddType<CharacterEndSee>(Commands.CMD_MC_CHAENDSEE);
            PacketToClass.AddType<ItemEndSee>(Commands.CMD_MC_ITEMENDSEE);
        }
    }
}
