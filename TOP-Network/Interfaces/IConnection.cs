using System.Net;
using TOP_Network.Interfaces;
using TOP_Network.Interfaces.Packets;

namespace TOP_Network;

public interface IConnection
{
    public bool IsServer { get; }
    public IPAddress IP { get; set; }
    public int Port { get; set; }

    public INetworkConnection?[] connections { get; }
    public Dictionary<uint, IRPacket?> Calls { get; }
    public uint PacketId { get; }

    public void Init(string IP = "", int port = 0);

    public Task StartAsServer();
    public Task StartAsClient(bool waitTillExit = false);

    public void Start();
    public Task OnConnected();
    public Task OnConnected(int socket);
    public Task<IPacket?> OnHandelPacket(IRPacket packet, int connection);
    public Task HandelPacket(IRPacket packet, int connection);
    public Task OnDisconect(int socket);

    public Task KeepAlive();

    public void Send(IPacket pkt, int connection = 0);
    public void SendToAll(IPacket pkt);

    public Task<IRPacket?> SyncCall(IPacket pkt, int timeOut = 10_000, int connection = 0);
    public void ReplyPacket(IRPacket originalPacket, IPacket sendPacket, int connection = 0);

    public int FindEmpty();

    public void Disconect(int connection = 0);
    public void DisconectAll();
    public bool IsConnected(int connection = 0);
}
