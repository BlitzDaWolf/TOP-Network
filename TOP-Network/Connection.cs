using Microsoft.Extensions.Logging;
using System.Net;
using System.Reflection;
using TOP_Network.Attributes;
using TOP_Network.Enum;
using TOP_Network.Exceptions;
using TOP_Network.Interfaces;
using TOP_Network.Interfaces.Packets;
using TOP_Network.Packets;
using TOP_Utils;

namespace TOP_Network;

public abstract class Connection<T> : IConnection where T : IConnection
{
    protected readonly ILogger<Connection<T>> _logger;
    private readonly IConectionFactory _connectionFactory;

    public bool IsServer { get; private set; } = false;
    public IPAddress IP { get; set; } = IPAddress.Any;
    public int Port { get; set; }

    public INetworkConnection?[] connections { get; private set; }

    public Dictionary<uint, IRPacket?> Calls { get; private set; } = new Dictionary<uint, IRPacket?>();

    private Task? RunningLoop;

    protected Connection(ILogger<Connection<T>> logger, IConectionFactory connectionFactory, int maxClients = 10)
    {
        connections = new INetworkConnection?[maxClients];
        _logger = logger;
        this._connectionFactory = connectionFactory;

        NetworkCommand<T>.InitCommands();
    }

    public uint PacketId { get; private set; }

    public void Init(string IP = "", int port = 0)
    {
        if (string.IsNullOrEmpty(IP)) throw new InvalidIPInitException("Empty IP was given");
        if (port <= 0) throw new InvalidPortInitException("Not an valid port was given");

        this.Port = port;
        if (!IPAddress.TryParse(IP, out var Ip))
        {
            IPHostEntry resolved = Dns.GetHostEntry(IP);
            if (resolved.AddressList.Length == 0) throw new InvalidPortInitException("Not an valid port was given");
            Ip = Dns.GetHostEntry(IP).AddressList.FirstOrDefault()!;
        }

        this.IP = Ip;

        Start();

        var connectionAttribute = GetType().GetCustomAttribute<ConnectionAttribute>();
        if (connectionAttribute != null)
        {
            switch (connectionAttribute)
            {
                case ServerAttribute:
                    _ = StartAsServer();
                    break;
                case ClientAttribute attribute:
                    StartAsClient(attribute.Wait).Wait();
                    break;
            }
        }
    }

    public async Task StartAsServer()
    {
        using var serverActivity = this.StartActivity("server start activity");
        if (Port == 0) throw new Exception("Server has not been initialized");
        IsServer = true;

        _connectionFactory.StartListener(IP, Port);
        try
        {
            _ = KeepAlive();
            _logger.LogInformation("Listening in: {ipaddress}:{port}", IP, Port);
            serverActivity?.Stop();
            while (true)
            {
                var client = await _connectionFactory.AcceptConnection();
                _ = Connect(client);
            }
        }
        finally
        {
        }
    }

    public async Task StartAsClient(bool waitTillExit = false)
    {
        if (IP == IPAddress.Any || Port == 0) throw new Exception("Client has not been initialized");
        INetworkConnection client = _connectionFactory.CreateConnection(IP, Port);
        if (waitTillExit)
        {
            await Connect(client);
        }
        else
        {
            _ = Connect(client);
        }

        await Task.Delay(1);
    }

    public virtual void Start() { }
    public virtual Task OnConnected() => Task.CompletedTask;
    public virtual Task OnConnected(int socket) => Task.CompletedTask;
    public virtual void OnPreHandel(IRPacket packet, int connection, IMethodBag bag) { }
    public virtual Task<IPacket?> OnHandelPacket(IRPacket packet, int connection)
    {
        _logger.LogInformation("Function not overwritten: [{functionName}]@{socket}", packet.Command, connection);
        return Task.FromResult<IPacket?>(null);
    }
    public virtual Task OnDisconect(int socket) => Task.CompletedTask;

    public async Task HandelPacket(IRPacket packet, int connection)
    {
        if (packet.Size == 2)
        {
            if (!IsServer) Send(packet, connection);
            return;
        }
        if (Calls.ContainsKey(packet.GNACK))
        {
            Calls[packet.GNACK] = packet;
            return;
        }

        if (!NetworkCommand<T>.TryHandlePacket((T)(IConnection)this, packet, connection, out var replyPacket, OnPreHandel))
        {
            replyPacket = await OnHandelPacket(packet, connection);
        }
        if (replyPacket != null)
        {
            ReplyPacket(packet, replyPacket, connection);
        }
    }

    public async Task KeepAlive()
    {
        IPacket p = new Packet();
        p.Init([0x00, 0x02]);
        while (true)
        {
            try
            {
                await Task.Delay(2000);
                for (int i = 0; i < connections.Length; i++)
                {
                    if (!IsConnected(i)) continue;
                    Send(p, i);
                }
            }
            finally
            {
            }
        }
    }

    private async Task Connect(INetworkConnection client)
    {
        await Task.Delay(100);
        _logger.LogInformation("Connection has been made");

        var emptySpot = FindEmpty();
        if (emptySpot == -1)
        {
            _logger.LogInformation("Max clients hit");
            return;
            // throw new Exception("No empty spots where found");
        }


        try
        {
            connections[emptySpot] = client;

            using (var act = this.StartActivity("On connected"))
            {
                _ = OnConnected();
                _ = OnConnected(emptySpot);
            }

            client.OnPacketRecive += (pkt) => _ = HandelPacket(pkt, emptySpot);
            var a = Task.WaitAny(client.ReciveLoop(), client.SendLoop());

        }
        finally
        {
            _logger.LogInformation("Connection [{socket}] has been closed", emptySpot);
            client.Close();
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
        if (!IsConnected(connection)) return; // Connection is disconnected 
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
        pkt.WriteGnack(++PacketId);

        var test = pkt.GNACK + 2147483648;

        Calls.Add(test, null);
        Send(pkt, connection);

        var delay = Task.Delay(timeOut);
        while (Calls[test] == null && !delay.IsCompleted) await Task.Delay(1);

        var result = Calls[test];
        Calls.Remove(test);

        return result;
    }
    public void ReplyPacket(IRPacket originalPacket, IPacket sendPacket, int connection = 0)
    {
        using var ReplayPacket = this.StartActivity("Replaying packet");

        ReplayPacket?.SetTag("Connection", connection);
        ReplayPacket?.SetTag("OGCommand", originalPacket.Command);

        sendPacket.WriteGnack(originalPacket.GNACK + 2147483648);
        Send(sendPacket, connection);
    }

    public void Disconect(int connection = 0)
    {
        if (!IsConnected(connection)) return; // Already disconnected 
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
