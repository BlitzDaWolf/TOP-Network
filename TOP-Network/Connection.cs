using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using TOP_Network.Interfaces.Packets;
using TOP_Network.Packets;
using TOP_Utils;

namespace TOP_Network;

public class Connection : IConnection
{
    private readonly ILogger<Connection> _logger;

    public bool IsServer { get; private set; } = false;
    public IPAddress IP { get; set; } = IPAddress.Any;
    public int Port { get; set; }

    public NetworkConnection?[] connections { get; private set; }

    public Dictionary<uint, IRPacket?> Calls { get; private set; } = new Dictionary<uint, IRPacket?>();

    public Connection (ILogger<Connection> logger, int maxClients = 10)
    {
        connections = new NetworkConnection?[maxClients];
        _logger = logger;
    }

    public uint PacketId { get; private set; }

    public void Init(string IP = "", int port = 0)
    {
        if (string.IsNullOrEmpty(IP) || port <= 0)
        {
            _logger.LogDebug("custom init Exception");
            throw new Exception("Not an valid IP or port was given");
        }

        this.Port = port;
        this.IP = IPAddress.Parse(IP);

        Thread t = new Thread(Start);
        t.Start();
    }

    public async Task StartAsServer()
    {
        IsServer = true;
        try
        {
            TcpListener listener = new TcpListener(IP, Port);
            listener.Start();

            _logger.LogInformation("Listening in: {0}:{1}", IP, Port);

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                _ = Task.Run(() => Connect(client));
            }
        }
        catch (Exception e)
        {
            _logger.LogError($"Error for listening: {IP}:{Port}");
            _logger.LogTrace(e.ToString());
        }
    }

    public async Task StartAsClient()
    {
        while (true)
        {
            try
            {
                _logger.LogInformation("Trying to connect to {0}:{1}", IP, Port);
                IsServer = false; // Ensure the flag is set to false
                TcpClient client = new TcpClient(IP.ToString(), Port);
                await Task.Run(() => Connect(client));
                await Task.Delay(500);
            }
            catch { }
            await Task.Delay(TimeSpan.FromSeconds(15));
        }
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

        var result = await OnHandelPacket(packet, connection);
        throw new NotImplementedException();
        // if (result != null) Send(packet, connection);
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
                var stream = connections[i]!.Value!.Stream;
                await stream.WriteAsync(p.Data, 0, p.Size);
                await stream.FlushAsync();
            }
        }
    }

    public async Task Connect(TcpClient Client)
    {
        await Task.Delay(1);
        _logger.LogDebug("Implement connection logic");
        var emptyValue =  FindEmpty();
        if (emptyValue == -1)
        {
            _logger.LogWarning("No valid place found");
            return;
        }
        _logger.LogDebug("Find empty spot");
        connections[emptyValue] = new NetworkConnection
        {
            Client = Client,
            ReciveBuffer = new NetworkBuffer(),
            SendBuffer = new NetworkBuffer(),
            Stream = Client.GetStream()
        };
        NetworkConnection currentConenction = connections[emptyValue]!.Value;


        using (var connecting = this.StartActivity("OnConnect"))
        {
            _ = OnConnected();
            _ = OnConnected(emptyValue);
        }

        try
        {
            _logger.LogDebug("Start reciving loop");
            _ = Task.Run(async () =>
            {
                try
                {
                    byte[] buffer = new byte[32_768];
                    int bytesRead = 0;
                    while ((bytesRead = await currentConenction.Stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        currentConenction.ReciveBuffer.AddData(buffer.Take(bytesRead));
                    }
                }
                catch { }


            });

            _logger.LogDebug("Start handle/send loop");
            int cnt = 0;
            while (IsConnected(emptyValue))
            {
                int hasData = (currentConenction.ReciveBuffer.EOF ? 0 : 1) + (currentConenction.SendBuffer.EOF ? 0 : 2);
                if (hasData >= 2)
                {
                    currentConenction.Stream.Write(currentConenction.SendBuffer.ReadAll());
                    currentConenction.Stream.Flush();
                    currentConenction.SendBuffer.SafeStep();
                }
                if (hasData == 0)
                {
                    await Task.Delay(10);
                }
                else if (hasData % 2 == 1)
                {
                    var currentPacket = currentConenction.ReciveBuffer.ReadPacket();
                    if (currentPacket.Size < currentPacket.StartSize) { } // Invalid packet skip
                    else if (currentPacket.Size == currentPacket.StartSize)
                    {
                        throw new NotImplementedException();
                        // if (IsServer) Send(currentPacket, emptyValue);
                    }
                    else
                    {
                        cnt++;
                        cnt %= 10;
                        _ = HandelPacket(currentPacket, emptyValue);

                        if (cnt == 0)
                        {
                            currentConenction.ReciveBuffer.SafeStep();
                        }
                    }
                }
            }
        }
        catch
        {

        }
        finally
        {
            _logger.LogDebug("Close out connection {0}", emptyValue);
            await OnDisconect(emptyValue);
            currentConenction.Close();
            connections[emptyValue] = null;
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
        throw new NotImplementedException();
        /*if (pkt.Size < 0) return; // Invalid packet Size
        using var SendActivity = this.StartActivity("Sending packet");
        SendActivity?.SetTag("Size", pkt.Size);
        SendActivity?.SetTag("Encrypted", false); // Todo implement encryption function

        if (!IsConnected(connection)) return;
        var currentConnection = connections[connection]!.Value;


        if (pkt.Size < 6 + Packet.StartSize)
        {
            // Ping
            SendActivity?.SetTag("Command", "PING");
            SendActivity?.SetTag("Ping", true);
        }
        else
        {
            SendActivity?.SetTag("Command", pkt.Command);
            SendActivity?.SetTag("Ping", false);
            if (currentConnection.SendTrace)
            {
                // If shareActivity
                var wpk = new WPacket(pkt);
                if (SendActivity != null)
                {
                    var id = SendActivity?.Id!.ToString()!;

                    var sz = wpk.WriteString(id);
                    wpk.WriteShort((short)(sz));
                    pkt = wpk.Clone();
                }
            }
        }

        if (connections[connection] is not null)
        {
            connections[connection]!.Value.SendBuffer.AddData(pkt);
        }
        else
        {
            _logger.LogWarning("[{1}] No conenction has been made at socket: {0}", connection, GetType().Name);
        }

        pkt.Final();
        */
    }

    public void SendToAll(IRPacket pkt)
    {
        using var SendToAll = this.StartActivity("Send to all");
        for (int i = 0; i < connections.Length; i++)
        {
            if (IsConnected(i)) Send(pkt, i);
        }
    }

    public async Task<IRPacket?> SyncCall(IRPacket pkt, int timeOut = 10_000, int connection = 0)
    {
        using var SyncActivity = this.StartActivity("Sync call");
        SyncActivity?.SetTag("Conenction", connection);
        SyncActivity?.SetTag("Command", pkt.Command);
        // pkt.AddRandomGnack();
        pkt.WriteGnack(++PacketId);
        uint test = pkt.GNACK + 2147483648;

        Send(pkt, connection);
        Calls.Add(test, null);
        var delay = Task.Delay(timeOut);
        using var WaitForReply = this.StartActivity("Sync wait");
        while (Calls[test] == null && !delay.IsCompleted)
        {
            await Task.Delay(1);
        }
        var result = Calls[test];
        Calls.Remove(test);
        if (result == null)
            WaitForReply?.SetStatus(ActivityStatusCode.Error);
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
        if (connections[connection] is null) return;
        connections[connection]!.Value.Close();
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