using System.Net.Sockets;
using TOP_Network.Converter;
using TOP_Network.Packets;
using TOP_Packets.Server;

public class Program
{
    public static void Main(string[] args)
    {
        // CMD_MC_FUNCPAGE
        var missions = Directory.GetFiles(@"D:\dev\DecryptFinal\DecryptFinal\bin\Debug\net8.0\packets\CMD_MC_MISLOG")
            .Select(File.ReadAllBytes)
            .Select(x => new Packet(x))
            .Select(x => x.Convert<MissionLog>())
            .ToList();

        var missionPage = Directory.GetFiles(@"D:\dev\DecryptFinal\DecryptFinal\bin\Debug\net8.0\packets\CMD_MC_MISPAGE")
            .Select(File.ReadAllBytes)
            .Select(x => new Packet(x))
            .Select(x => x.Convert<MissionPage>())
            .ToList();
        var missionLogInfo = Directory.GetFiles(@"D:\dev\DecryptFinal\DecryptFinal\bin\Debug\net8.0\packets\CMD_MC_MISLOGINFO")
            .Select(File.ReadAllBytes)
            .Select(x => new Packet(x))
            .Select(x => x.Convert<MissionLogInfo>())
            .ToList();

        var NpcStateChange = Directory.GetFiles(@"D:\dev\DecryptFinal\DecryptFinal\bin\Debug\net8.0\packets\CMD_MC_NPCSTATECHG")
            .Select(File.ReadAllBytes)
            .Select(x => new Packet(x))
            .Select(x => x.Convert<NpcStateChange>())
            .ToList();

    }
}