using Microsoft.Extensions.Logging;
using System.Configuration.Assemblies;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
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
    private readonly IConectionFactory conectionFactory;

    public bool IsServer { get; private set; } = false;
    public IPAddress IP { get; set; } = IPAddress.Any;
    public int Port { get; set; }

    public INetworkConnection?[] connections { get; private set; }

    public Dictionary<uint, IRPacket?> Calls { get; private set; } = new Dictionary<uint, IRPacket?>();

    private Task? RunningLoop;

    public Connection(ILogger<Connection<T>> logger, IConectionFactory conectionFactory, int maxClients = 10)
    {
        connections = new INetworkConnection?[maxClients];
        _logger = logger;
        this.conectionFactory = conectionFactory;

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
            if (connectionAttribute is ServerAttribute) _ = StartAsServer();
            if (connectionAttribute is ClientAttribute) StartAsClient(((ClientAttribute)connectionAttribute).Wait).Wait();
        }
    }

    public async Task StartAsServer()
    {
        if (IP == IPAddress.Any || Port == 0) throw new Exception("Server has not been initialized");
        IsServer = true;

        conectionFactory.StartListener(IP, Port);
        try
        {
            List<Task> connections = new List<Task>();

            _ = KeepAlive();
            _logger.LogInformation("Listening in: {0}:{1}", IP, Port);
            while (true)
            {
                var client = await conectionFactory.AcceptConnection();
                _ = Connect(client);
            }
        }
        catch (Exception e)
        {

        }

        var enbd = "";
    }

    public async Task StartAsClient(bool waitTillExit = false)
    {
        if (IP == IPAddress.Any || Port == 0) throw new Exception("Client has not been initialized");
        INetworkConnection client = conectionFactory.CreateConnection(IP, Port);
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
    public virtual void OnPreHandel(IRPacket packet, int connection, IMethodBag Bag) { }
    public virtual Task<IPacket?> OnHandelPacket(IRPacket packet, int connection)
    {
        _logger.LogInformation("Function not overwriten: [{0}]@{1}", packet.Command, connection);
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

        if(!NetworkCommand<T>.TrHandlePacket((T)(IConnection)this, packet, connection, out IPacket? replyPacket, OnPreHandel))
        {
            replyPacket = await OnHandelPacket(packet, connection);
            if (replyPacket != null)
            {
                ReplyPacket(packet, replyPacket, connection);
            }
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
                // foreach (var a in connections)
                for (int i = 0; i < connections.Length; i++)
                {
                    if (!IsConnected(i)) continue;
                    Send(p, i);
                }
            }
            catch
            {

            }
        }
    }

    public async Task Connect(INetworkConnection Client)
    {
        await Task.Delay(100);
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
        catch (Exception e) { }
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


    public IPacket RequestCommands()
    {
        var methods = GetType().GetMethods().Where(x => x.GetCustomAttribute<PacketHandleAttribute>() != null).ToArray();

        IWPacket wpk = new WPacket();
        wpk.WriteCommand(Commands.CMD_UU_COMMANDS);
        wpk.WriteShort((short)Errors.ERR_SUCCESS);

        wpk.WriteShort((short)methods.Length);
        for (int i = 0; i < methods.Length; i++)
        {
            var m = methods[i];
            var handeler = m.GetCustomAttribute<PacketHandleAttribute>()!.CommandType;

            wpk.WriteShort((short)handeler);
            wpk.WriteString(handeler.ToString());
        }

        return wpk;
    }
}
