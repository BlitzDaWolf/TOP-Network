using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using TOP_Network.Exceptions;
using TOP_Network.Interfaces;
using TOP_Network.Interfaces.Packets;
using TOP_Network.Packets;
using TOP_Utils;

namespace TOP_Network;

public class Connection : IConnection
{
    protected readonly ILogger<Connection> _logger;
    private readonly IConectionFactory conectionFactory;

    public bool IsServer { get; private set; } = false;
    public IPAddress IP { get; set; } = IPAddress.Any;
    public int Port { get; set; }

    public INetworkConnection?[] connections { get; private set; }

    public Dictionary<uint, IRPacket?> Calls { get; private set; } = new Dictionary<uint, IRPacket?>();

    public Connection(ILogger<Connection> logger, IConectionFactory conectionFactory, int maxClients = 10)
    {
        connections = new INetworkConnection?[maxClients];
        _logger = logger;
        this.conectionFactory = conectionFactory;
    }

    public uint PacketId { get; private set; }

    public void Init(string IP = "", int port = 0)
    {
        if (string.IsNullOrEmpty(IP)) throw new InvalidIPInitException("Empty IP was given");
        if (port <= 0) throw new InvalidPortInitException("Not an valid port was given");

        this.Port = port;
        this.IP = IPAddress.Parse(IP);
    }

    public async Task StartAsServer()
    {
        if (IP == IPAddress.Any || Port == 0) throw new Exception("Server has not been initialized");
        IsServer = true;

        new Thread(async () =>
        {
            conectionFactory.StartListener(IP, Port);

            _logger.LogInformation("Listening in: {0}:{1}", IP, Port);

            while (true)
            {
                var client = await conectionFactory.AcceptConnection();
                new Thread(async () => await Connect(client)).Start();
            }
        }).Start();

        await Task.Delay(1);
    }

    public async Task StartAsClient()
    {
        if (IP == IPAddress.Any || Port == 0) throw new Exception("Client has not been initialized");
        INetworkConnection client = conectionFactory.CreateConnection();
        new Thread(async () => await Connect(client)).Start();
        await Task.Delay(1);
    }

    public virtual void Start() { }
    public virtual Task OnConnected() => Task.CompletedTask;
    public virtual Task OnConnected(int socket) => Task.CompletedTask;
    public virtual Task<IPacket?> OnHandelPacket(IRPacket packet, int connection)
    {
        _logger.LogInformation("Function not overwriten: [{0}]@{1}", packet.Command, connection);
        return Task.FromResult<IPacket?>(null);
    }
    public virtual Task OnDisconect(int socket) => Task.CompletedTask;

    public async Task HandelPacket(IRPacket packet, int connection)
    {
        if (Calls.ContainsKey(packet.GNACK))
        {
            Calls[packet.GNACK] = packet;
            return;
        }

        var replyPacket = await OnHandelPacket(packet, connection);
        if (replyPacket != null)
        {
            ReplyPacket(packet, replyPacket, connection);
        }
    }

    public async Task KeepAlive()
    {
        IPacket p = new Packet();
        p.Init([ 0x00, 0x02 ]);
        while (true)
        {
            await Task.Delay(2000);
            // foreach (var a in connections)
            for (int i = 0; i < connections.Length; i++)
            {
                if (!IsConnected(i)) continue;
                
            }
        }
    }

    public async Task Connect(INetworkConnection Client)
    {
        _logger.LogInformation("Conenction has been made");

        var emptySpot = FindEmpty();
        if (emptySpot == -1)
        {
            _logger.LogInformation("Max clients hit");
            return;
            // throw new Exception("No empty spots where found");
        }


        try
        {
            connections[emptySpot] = Client;

            using (var act = this.StartActivity("On connected"))
            {
                _ = OnConnected();
                _ = OnConnected(emptySpot);
            }

            Client.OnPacketRecive += (pkt) => _ = HandelPacket(pkt, emptySpot);
            var a = Task.WaitAny(Client.ReciveLoop(), Client.SendLoop());

        }
        catch (Exception e)
        {

        }
        finally
        {
            _logger.LogInformation("Connection [{0}] has been closed", emptySpot);
            Client.Close();
            connections[emptySpot] = null;
            await OnDisconect(emptySpot);
        }
    }

    public int FindEmpty()
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (!IsConnected(i)) return i;
        }
        return -1;
    }

    public void Send(IPacket pkt, int connection)
    {
        if (!IsConnected(connection)) return; // Connection is disconected 
        connections[connection]!.SendBuffer.AddData(pkt);
    }

    public void SendToAll(IPacket pkt)
    {
        using var SendToAll = this.StartActivity("Send to all");
        for (int i = 0; i < connections.Length; i++)
        {
            if (IsConnected(i)) Send(pkt, i);
        }
    }

    public async Task<IRPacket?> SyncCall(IPacket pkt, int timeOut = 10_000, int connection = 0)
    {
        return null;
    }
    public void ReplyPacket(IRPacket originalPacket, IPacket sendPacket, int connection = 0)
    {
        using var ReplayPacket = this.StartActivity("Replaying packet");

        ReplayPacket?.SetTag("Conenction", connection);
        ReplayPacket?.SetTag("OGCommand", originalPacket.Command);

        sendPacket.WriteGnack(originalPacket.GNACK + 2147483648);
        Send(sendPacket, connection);
    }

    public void Disconect(int connection = 0)
    {
        if (!IsConnected(connection)) return; // Already disconected 
        connections[connection]!.Close();
        connections[connection] = null;
    }
    public void DisconectAll()
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (IsConnected(i)) Disconect(i);
        }
    }
    public bool IsConnected(int connection = 0) => this.connections[connection] is not null;
}

[ExcludeFromCodeCoverage]
public class Connection<T> where T : IConnection
{
    public static T Instance { get => _instance ?? throw new Exception("Instance has not been set"); }
    private static T? _instance;// = new T();

    public static void SetInstance(T i)
    {
        if (_instance != null) return; _instance = i;
    }

    public static void Send(IRPacket pkt, int connection = 0) => Instance.Send(pkt, connection);
    public static void SendToAll(IRPacket pkt) => Instance.SendToAll(pkt);
    public static void Init(string ip="", int port =0) => Instance.Init(ip, port);

    public static void Disconect(int socket) => Instance.Disconect(socket);

    public static Task<IRPacket?> SyncCall(IRPacket wpk, int timeOut = 1_000) => Instance.SyncCall(wpk, timeOut);

    public static void DisconectAll() => Instance.DisconectAll();
}