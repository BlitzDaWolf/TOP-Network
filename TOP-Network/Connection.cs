using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using TOP_Network.Packets;

namespace TOP_Network;

public abstract class Connection
{
    private TcpClient?[] connections;
    private NetworkStream?[] sockets;
    // TcpClient? tcpClient;
    // NetworkStream? stream;

    private Dictionary<uint, Packet?> called = new Dictionary<uint, Packet?>();

    public Connection(int maxConnections = 10)
    {
        connections = new TcpClient[maxConnections];
        sockets = new NetworkStream[maxConnections];
    }

    public void Init()
    {
        Start();
    }

    public virtual void Start() { }

    public async Task KeepAlive()
    {
        /*Packet p = new Packet(new byte[]{ 0x00, 0x02 });
        while (tcpClient != null && tcpClient.Connected)
        {
            await Task.Delay(2000);
            // await Send(p);
            await stream.WriteAsync(p.Data, 0, p.Size);
            await stream.FlushAsync();
        }*/
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
        if(empty == -1)
        {
            Logging.LogInfo("Max connections reached");
            groupServer.Close();
            return;
        }
        connections[empty] = groupServer;
        sockets[empty] = groupServer.GetStream();

        // var kal = KeepAlive();

        try
        {
            var _ = OnConnected();
            bool IsTransaction = false;

            List<byte> allData = new List<byte>();

            byte[] buffer = new byte[32_768];
            int bytesRead = 0;
            while  ((bytesRead = await sockets[empty]!.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                // bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (IsTransaction)
                {
                    allData.AddRange(buffer);
                }
                else
                {
                    var t = new List<byte>(buffer.Take(bytesRead));
                    List<Packet> packets = new List<Packet>();
                    while(t.Count > 1)
                    {
                        var packetSize = new Packet(t.Take(Packet.StartSize).ToArray());
                        if(packetSize.Size == 0)
                        {
                            t.RemoveRange(0, 1);
                            continue;
                        }
                        if(packetSize.Size < 8 || t.Count < 8)
                        {
                            // Check
                            if(packetSize.Size == Packet.StartSize)
                            {
                                await Send(packetSize, empty);
                            }
                            t.RemoveRange(0, 1);
                            continue;
                        }
                        packets.Add(new Packet(t.Take(packetSize.Size).ToArray()));
                        t.RemoveRange(0, packetSize.Size);
                    }
                    Task.WaitAll(packets.Select(x=>handelPacket(x, empty)).ToArray());
                }
                IsTransaction = false;
            }
        }
        catch(Exception e)
        {
            Logging.LogError(e);
        }
        finally
        {
            Logging.LogInfo($"Closing connection [{empty}]");
            sockets[empty]!.Close();
            connections[empty]!.Close();
        }

        sockets[empty] = null;
        connections[empty] = null;
    }

    private async Task handelPacket(Packet pkt, int connection)
    {
        if (called.Count != 0)
        {
            if (called.ContainsKey(pkt.gnack))
            {
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

    public virtual Task OnConnected() => Task.CompletedTask;
    public virtual Task<Packet?> HandelPacket(Packet pkt, int socketNr) => Task.FromResult<Packet?>(null);

    public async Task Send(Packet pkt, int conenction = 0)
    {
        if (pkt.Size >= 10)
        {
            Logging.LogInfo($"Sending: [{pkt.Size}]");
        }
        await sockets[conenction]!.WriteAsync(pkt.Data, 0, (int)pkt.Size);
        await sockets[conenction]!.FlushAsync();
        pkt.Final();
    }

    public uint packet { get; private set; } = 0;

    public async Task<Packet?> SyncCall(Packet pkt, int timeout = 10_000)
    {
        // pkt.AddRandomGnack();
        pkt.WriteNewGnack(++packet);
        uint test = pkt.gnack + 2147483648;

        await Send(pkt);
        called.Add(test, null);
        var delay = Task.Delay(timeout);
        while (called[test] == null && !delay.IsCompleted)
        {
            await Task.Delay(100);
        }
        var result = called[test];
        called.Remove(test);
        return result;
    }

    public async Task ReplyPacket(Packet pkt, Packet packet, int connection)
    {   
        packet.WriteNewGnack(pkt.gnack + 2147483648);
        await Send(packet, connection);
    }

    public async void SendToAll(Packet packet)
    {
        for (int i = 0; i < sockets.Length; i++)
        {
            if (sockets[i] != null)
                await Send(packet, i);
        }
    }
}
