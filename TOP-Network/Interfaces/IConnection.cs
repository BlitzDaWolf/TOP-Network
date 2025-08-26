using System;
using System.Net;
using TOP_Network.Packets;

namespace TOP_Network;

public interface IConnection
{
    public bool IsServer { get; }
    public IPAddress IP { get; set; }
    public int Port { get; set; }

    public NetworkConnection?[] connections { get; }
    public Dictionary<uint, RPacket?> Calls { get; }
    public uint PacketId { get; }

    public void Init(string IP = "", int port = 0);

    public Task StartAsServer();
    public Task StartAsClient();

    public void Start();
    public Task OnConnected();
    public Task OnConnected(int socket);
    public Task<V1Packet?> OnHandelPacket(RPacket packet, int connection);
    public Task HandelPacket(RPacket packet, int connection);
    public Task OnDisconect(int socket);

    public Task KeepAlive();

    public void Send(V1Packet pkt, int connection = 0);
    public void SendToAll(V1Packet pkt);

    public Task<RPacket?> SyncCall(V1Packet pkt, int timeOut = 10_000, int connection = 0);
    public void ReplyPacket(V1Packet originalPacket, V1Packet sendPacket, int connection = 0);

    public int FindEmpty();

    public void Disconect(int connection = 0);
    public void DisconectAll();
    public bool IsConnected(int connection = 0);
}
