using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using TOP_Network.Packets;
using TOP_Utils;

namespace TOP_Network;



public abstract class ConnectionV1
{
    public bool IsServer { get; set; }
    public string IP { get; private set; } = "";
    public int Port { get; private set; }


    private NetworkConnection?[] connections;
    // private TcpClient?[] connections;
    // private NetworkStream?[] sockets;
    // TcpClient? tcpClient;
    // NetworkStream? stream;

    private Dictionary<uint, V1Packet?> called = new Dictionary<uint, V1Packet?>();

    public ConnectionV1(int maxConnections = 10)
    {
        connections = new NetworkConnection?[maxConnections];
        // connections = new TcpClient[maxConnections];
        // sockets = new NetworkStream[maxConnections];
    }

    public void Init(string IP = "", int port = 0)
    {
        if (string.IsNullOrEmpty(IP) || port <= 0)
        {
            Logging.LogError("No valid IP or port was given");
            return;
        }

        this.IP = IP;
        this.Port = port;

        Thread t = new Thread(Start);
        t.Start();
    }

    public virtual void Start() { }

    public async Task KeepAlive()
    {
        V1Packet p = new V1Packet(new byte[] { 0x00, 0x02 });
        while (true)
        {
            await Task.Delay(2000);
            foreach (var a in connections)
            {
                var stream = a.Value.Stream;
                // await Send(p);
                await stream.WriteAsync(p.Data, 0, p.Size);
                await stream.FlushAsync();
            }
        }
    }

    public void Disconect(int socket = 0)
    {
        if (connections[socket] == null) return;
        connections[socket]!.Value.Close();
    }

    public async Task ConnectAsClient(string IP, int port)
    {
        TcpClient client = new TcpClient(IP, port);
        await connect(client);
    }

    /// <summary>
    /// Only allows 1 client to connect to the server
    /// </summary>
    /// <param name="listen">Listing IP address, `0.0.0.0`</param>
    /// <param name="port">Listen port</param>
    public async Task RunAsSingleServer(IPAddress listen, int port)
    {
        IsServer = true;
        try
        {
            TcpListener listener = new TcpListener(listen, port);
            listener.Start();

            Logging.LogImportant($"Listening on: {listen}:{port}");

            while (true)
            {
                TcpClient GroupServer = listener.AcceptTcpClient();
                _ = Task.Run(() => connect(GroupServer));
                Logging.LogInfo("Client connected");
            }
        }
        catch (Exception e)
        {
            Logging.LogError($"Error for listening: {listen}:{port}");
            Logging.LogError(e);
        }
    }

    public int FindEmpty()
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (connections[i] == null)
                return i;
        }
        return -1;
    }

    public async Task connect(TcpClient groupServer)
    {
        int empty = FindEmpty();
        if (empty == -1)
        {
            Logging.LogInfo("Max connections reached");
            groupServer.Close();
            return;
        }
        var con = new NetworkConnection
        {
            Client = groupServer,
            Stream = groupServer.GetStream(),
            ReciveBuffer = new NetworkBuffer(),
            SendBuffer = new NetworkBuffer(),
            SendTrace = false
        };
        connections[empty] = con;

        try
        {
            using (var onConnection = this.StartActivity("OnConnecting"))
            {
                onConnection?.SetTag("Socket number", empty);
                onConnection?.SetTag("Connection class", GetType());
                _ = OnConnected();
                _ = OnConnected(empty);
            }

            {
                if (IsServer)
                {
                    var tmp = new WPacket();
                    tmp.WriteCMD(Enum.Commands.CMD_UU_SYNC);
                    tmp.WriteSize(6 + V1Packet.StartSize);
                    await Send(tmp);
                }
            }

            if (!con.Client.Connected)
            {
                goto Final;
            }

            var reciveLoop = new Task(async () =>
            {
                try
                {
                    byte[] buffer = new byte[32_768];
                    int bytesRead = 0;
                    while ((bytesRead = await connections[empty]!.Value.Stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        /*if (bytesRead < Packet.StartSize + 8)
                        {
                            await Send(new Packet(buffer.Take(bytesRead).ToArray()), empty);
                        }
                        else*/
                        {
                            connections[empty]!.Value.ReciveBuffer.AddData(buffer.Take(bytesRead));
                        }
                    }
                }
                catch { }

                groupServer.Close();
            });
            reciveLoop.Start();
            // HandelLoop.Start();
            int cnt = 0;

            while (con.Client.Connected)
            {
                int hasData = (con.ReciveBuffer.EOF ? 0 : 1) + (con.SendBuffer.EOF ? 0 : 2);

                if (hasData >= 2)
                {
                    con.Stream.Write(con.SendBuffer.ReadAll());
                    con.Stream.Flush();
                    con.SendBuffer.SafeStep();
                }
                if (hasData == 0)
                {
                    await Task.Delay(10);
                }
                else if (hasData % 2 == 1)
                {
                    var currentPacket = con.ReciveBuffer.ReadPacket();
                    if (currentPacket.Size < V1Packet.StartSize) { } // Invalid packet skip
                    else if (currentPacket.Size == V1Packet.StartSize)
                    {
                        if (IsServer) await Send(currentPacket, empty);
                    }
                    else
                    {
                        cnt++;
                        cnt %= 10;
                        _ = handelPacket(currentPacket, empty);

                        if (cnt == 0)
                        {
                            con.ReciveBuffer.SafeStep();
                        }
                    }
                }
            }

            /*while (groupServer.Connected)
            {
                try
                {
                    Logging.LogInfo("{0}:{1}", GetType().FullName!, groupServer.Connected);
                    while (connections[empty]!.Value.ReciveBuffer.EOF)
                    {
                        await Task.Delay(1);
                    }
                    if (!groupServer.Connected) break;
                    var currentPacket = connections[empty]!.Value.ReciveBuffer.ReadPacket();
                    if (currentPacket.Size <= Packet.StartSize + 8)
                    {
                        if(!IsServer) _ = Send(currentPacket, empty);
                    }
                    else
                    {
                        _ = handelPacket(currentPacket, empty);
                    }
                }
                catch (Exception e)
                {
                    Logging.LogError(e);
                }
                cnt++;
                cnt %= 10;
                if (cnt == 0)
                    connections[empty]!.Value.ReciveBuffer.SafeStep();
                Logging.LogInfo("count: {0}", cnt);
            }*/
        }
        catch (ObjectDisposedException)
        {
            // Conenction posibly closed
        }
        catch (SocketException) { }
        catch (Exception e)
        {
            Logging.LogError(e);
        }
        finally
        {
            await OnDisconect(empty);
            Logging.LogInfo($"Closing connection [{empty}]");
            connections[empty]!.Value.Close();
        }

    Final:
        connections[empty] = null;
    }

    private async Task handelPacket(V1Packet pkt, int connection)
    {
        var con = connections[connection].Value;

        Logging.LogInfo("{0}", pkt.Command);
        if (pkt.Command == Enum.Commands.CMD_UU_SYNC)
        {
            con.SendTrace = true;
            if (!IsServer)
            {
                var tmp = new WPacket();
                tmp.WriteCMD(Enum.Commands.CMD_UU_SYNC);
                tmp.WriteSize(6 + V1Packet.StartSize);
                await Send(tmp);
            }
            return;
        }
        string? parentid = null;
        if (con.SendTrace)
        {
            try
            {
                // If shareActivity
                var rpk = pkt.GetRPacket();
                {
                    var str = rpk.ReverseReadString();
                    pkt.RemoveLast(str.Length + 5);
                    parentid = str;
                    // Logging.LogInfo("SID: {0}", str.Length);
                    // Logging.LogInfo("SID: {0}: {1}", str.Length, str);
                }
            }
            catch
            {

            }
        }
        using var _HandelPacket = this.StartActivity("Handeling packet", parentID: parentid);
        _HandelPacket?.SetTag("command", pkt.Command);
        _HandelPacket?.SetTag("Size", pkt.Size);
        _HandelPacket?.SetTag("Sync packet", false);
        try
        {
            if (called.Count != 0)
            {
                if (called.ContainsKey(pkt.gnack))
                {
                    _HandelPacket?.SetTag("Sync packet", true);
                    called[pkt.gnack] = pkt;
                    return;
                }
            }
            var p = await HandelPacket(pkt, connection);
            if (p != null)
            {
                await ReplyPacket(pkt, p, connection);
            }
        }
        catch (Exception ex)
        {
            Logging.LogError(ex);
            _HandelPacket?.SetTag("Error", true);
            _HandelPacket?.SetTag("Message", ex);
            throw;
        }
    }

    public virtual Task OnConnected() => Task.CompletedTask;
    public virtual Task OnConnected(int socket) => Task.CompletedTask;
    public virtual Task<V1Packet?> HandelPacket(V1Packet pkt, int socketNr) => Task.FromResult<V1Packet?>(null);
    public virtual Task OnDisconect(int socket) => Task.CompletedTask;

    public async Task Send(V1Packet pkt, int conenction = 0)
    {
        if (pkt.Size < 0) return; // Not a valid packet size

        using var SendActivity = this.StartActivity("Sending packet");
        SendActivity?.SetTag("Size", pkt.Size);

        if (pkt.Size < 6 + V1Packet.StartSize)
        {
            SendActivity?.SetTag("Command", "PING");
            SendActivity?.SetTag("Ping", true);
        }
        else
        {
            SendActivity?.SetTag("Command", pkt.Command);
            SendActivity?.SetTag("Ping", false);
            if (connections[conenction].Value.SendTrace)
            {
                // If shareActivity
                var wpk = new WPacket(pkt);
                if (SendActivity != null)
                {
                    var id = SendActivity?.Id.ToString()!;

                    var sz = wpk.WriteString(id);
                    wpk.WriteShort((short)(sz));
                    pkt = wpk.Clone();
                }
                else
                {

                }
            }
        }

        if (connections[conenction] != null)
        {
            connections[conenction]!.Value.SendBuffer.AddData(pkt);
            // await connections[conenction]!.Value.Stream.WriteAsync(pkt.Data, 0, (int)pkt.Size);
            // await connections[conenction]!.Value.Stream.FlushAsync();
        }
        else
        {
            Logging.LogInfo("[{1}] No conenction has been made at socket: {0}", conenction, GetType().Name);
        }
        pkt.Final();
    }

    public uint packet { get; private set; } = 0;

    public async Task<RPacket?> SyncCall(V1Packet pkt, int timeout = 10_000, int connection = 0)
    {
        using var SyncActivity = this.StartActivity("Sync call");
        SyncActivity?.SetTag("Conenction", connection);
        SyncActivity?.SetTag("Command", pkt.Command);
        // pkt.AddRandomGnack();
        pkt.WriteNewGnack(++packet);
        uint test = pkt.gnack + 2147483648;

        await Send(pkt);
        called.Add(test, null);
        var delay = Task.Delay(timeout);
        using var WaitForReply = this.StartActivity("Sync wait");
        while (called[test] == null && !delay.IsCompleted)
        {
            await Task.Delay(1);
        }
        var result = called[test];
        called.Remove(test);
        if (result == null)
            WaitForReply?.SetStatus(ActivityStatusCode.Error);
        return result?.GetRPacket();
    }

    public async Task ReplyPacket(V1Packet pkt, V1Packet packet, int connection)
    {
        using var ReplayPacket = this.StartActivity("Replaying packet");

        ReplayPacket?.SetTag("Conenction", connection);
        ReplayPacket?.SetTag("OGCommand", pkt.Command);

        packet.WriteNewGnack(pkt.gnack + 2147483648);
        await Send(packet, connection);
    }

    public async void SendToAll(V1Packet packet)
    {
        using var SendToAll = this.StartActivity("Send to all");
        for (int i = 0; i < connections.Length; i++)
        {
            if (connections[i] != null)
                await Send(packet, i);
        }
    }

    public int GetIP(int socket)
    {
        return 16777343; // loopback (127.0.0.1)
        // var i = this.connections[socket]!.Client.RemoteEndPoint as IPEndPoint;
        // return 16777343; // loopback (127.0.0.1)
    }

    internal void DisconectAll()
    {
        for (int i = 0; i < connections.Length; i++)
        {
            Disconect(i);
        }
    }
    
    

    public bool IsConnected(int connection = 0) => this.connections[connection] != null;
}

public class Connection<T> where T : IConnection
{
    public static T Instance { get => _instance ?? throw new Exception("Instance has not been set"); }
    private static T? _instance;// = new T();

    public static void SetInstance(T i)
    {
        if (_instance != null) return; _instance = i;
    }

    public static void Send(V1Packet pkt, int connection = 0) => Instance.Send(pkt, connection);
    public static void SendToAll(V1Packet pkt) => Instance.SendToAll(pkt);
    public static void Init(string ip="", int port =0) => Instance.Init(ip, port);

    public static void Disconect(int socket) => Instance.Disconect(socket);

    public static Task<RPacket?> SyncCall(V1Packet wpk, int timeOut = 1_000) => Instance.SyncCall(wpk, timeOut);

    public static void DisconectAll() => Instance.DisconectAll();
}