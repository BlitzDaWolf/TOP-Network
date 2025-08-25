using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using TOP_Network.Packets;
using TOP_Utils;

namespace TOP_Network;

public class Connection : IConnection
{
    public bool IsServer { get; private set; } = false;
    public IPAddress IP { get; set; } = IPAddress.Any;
    public int Port { get; set; }

    public NetworkConnection?[] connections { get; private set; }

    public Dictionary<uint, Packet?> Calls { get; private set; } = new Dictionary<uint, Packet?>();

    public Connection(int maxClients = 10)
    {
        connections = new NetworkConnection?[maxClients];
    }

    public uint PacketId { get; private set; }

    public void Init(string IP = "", int port = 0)
    {
        if (string.IsNullOrEmpty(IP) || port <= 0)
        {
            Logging.LogTodo("custom init Exception");
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

            Logging.LogInfo("Listening in: {0}:{1}", IP, Port);

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                _ = Task.Run(() => Connect(client));
            }
        }
        catch (Exception e)
        {
            Logging.LogError($"Error for listening: {IP}:{Port}");
            Logging.LogError(e);
        }
    }

    public async Task StartAsClient()
    {
        IsServer = false; // Ensure the flag is set to false
        TcpClient client = new TcpClient(IP.ToString(), Port);
        _ = Task.Run(() => Connect(client));
        await Task.Delay(500);
    }

    public virtual void Start() { }
    public virtual Task OnConnected() => Task.CompletedTask;
    public virtual Task OnConnected(int socket) => Task.CompletedTask;
    public virtual Task<Packet?> HandelPacket(RPacket packet, int connection) => Task.FromResult<Packet?>(null);
    public virtual Task OnDisconect(int socket) => Task.CompletedTask;

    public async Task KeepAlive()
    {
        Packet p = new Packet(new byte[] { 0x00, 0x02 });
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
        Logging.LogTodo("Implement connection logic");

        Logging.LogTodo("Find empty spot");

        try
        {
            Logging.LogTodo("Start reciving loop");

            Logging.LogTodo("Start handle/send loop");
        }
        catch
        {

        }
        finally
        {
            Logging.LogTodo("Close out connection");
        }

    }

    public void Send(Packet pkt, int connection)
    {
        if (pkt.Size < 0) return; // Invalid packet Size
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
            Logging.LogWarning("[{1}] No conenction has been made at socket: {0}", connection, GetType().Name);
        }

        pkt.Final();
    }

    public void SendToAll(Packet pkt)
    {
        using var SendToAll = this.StartActivity("Send to all");
        for (int i = 0; i < connections.Length; i++)
        {
            if (IsConnected(i)) Send(pkt, i);
        }
    }

    public async Task<RPacket?> SyncCall(Packet pkt, int timeOut = 10_000, int connection = 0)
    {
        using var SyncActivity = this.StartActivity("Sync call");
        SyncActivity?.SetTag("Conenction", connection);
        SyncActivity?.SetTag("Command", pkt.Command);
        // pkt.AddRandomGnack();
        pkt.WriteNewGnack(++PacketId);
        uint test = pkt.gnack + 2147483648;

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
        return result?.GetRPacket();
    }
    public void ReplyPacket(Packet originalPacket, Packet sendPacket, int connection = 0)
    {
        using var ReplayPacket = this.StartActivity("Replaying packet");

        ReplayPacket?.SetTag("Conenction", connection);
        ReplayPacket?.SetTag("OGCommand", originalPacket.Command);

        sendPacket.WriteNewGnack(originalPacket.gnack + 2147483648);
        Send(sendPacket, connection);
    }

    public void Disconect(int connection)
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
