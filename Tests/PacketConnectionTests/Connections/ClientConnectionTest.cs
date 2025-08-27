using Microsoft.Extensions.Logging;
using PacketConnectionTests.Abstractions;
using PacketConnectionTests.Abstractions.Facotries;
using TOP_Network;
using TOP_Network.Interfaces.Packets;
using TOP_Network.Packets;

namespace PacketConnectionTests.Connections;

public class ClientConnectionTest
{
    private readonly ILogger<TestClientConnection> TestLogger;

    public ClientConnectionTest()
    {
        TestLogger = LoggerFactory.Create(conf => conf.ClearProviders()).CreateLogger<TestClientConnection>();
    }

    [Fact]
    public async Task StartClientException()
    {
        
        IConnection connection = new TestClientConnection(TestLogger, new EmptyConnectionFactory());
        await Assert.ThrowsAsync<Exception>(() => connection.StartAsClient());
    }

    [Fact]
    public async Task StartClient()
    {
        IConnection connection = new TestClientConnection(TestLogger, new NormalConnectionFactory());
        connection.Init("127.0.0.1", 1234);

        Assert.Equal(1234, connection.Port);
        Assert.Equal("127.0.0.1", connection.IP.ToString());

        _ = connection.StartAsClient();
        await Task.Delay(TimeSpan.FromSeconds(0.5));

        Assert.False(connection.IsServer);
        Assert.True(connection.IsConnected());
    }

    [Fact]
    public async Task OnDisconect()
    {
        TestClientConnection connection = new TestClientConnection(TestLogger, new NormalConnectionFactory{timing = 1});
        connection.Init("127.0.0.1", 1234);

        Assert.Equal(1234, connection.Port);
        Assert.Equal("127.0.0.1", connection.IP.ToString());

        _ = connection.StartAsClient();
        await Task.Delay(TimeSpan.FromSeconds(0.5));

        Assert.False(connection.IsServer);
        Assert.True(connection.IsConnected());

        await Task.Delay(800);
        Assert.False(connection.IsConnected());
        Assert.Equal(1, connection.DisconnectedCall);
    }

    [Fact]
    public async Task ReadCorrectBuffer()
    {
        byte[] d = new byte[68];

        Random.Shared.NextBytes(d);
        d[0] = 0;
        d[1] = 68;


        IConnection connection = new TestClientConnection(TestLogger, null);
        connection.Init("127.0.0.1", 1234);

        Assert.Equal(1234, connection.Port);
        Assert.Equal("127.0.0.1", connection.IP.ToString());

        _ = connection.StartAsClient();
        await Task.Delay(TimeSpan.FromSeconds(1));

        Assert.False(connection.IsServer);
        Assert.True(connection.IsConnected());

        await Task.Delay(500);
        Assert.True(connection.IsConnected());

        var recvBuffer = connection.connections[0]!.ReciveBuffer;
        Assert.Equal(68, recvBuffer.Remaining);
        Assert.Equal(d, recvBuffer.ReadAll());
    }

    [Fact]
    public async Task ReadCorrectPacketBuffer()
    {
        IWPacket wpk = new WPacket();
        byte[] d = new byte[68];

        Random.Shared.NextBytes(d);
        wpk.WriteSeq(d);

        TestClientConnection connection = new TestClientConnection(TestLogger, null);
        connection.Init("127.0.0.1", 1234);

        Assert.Equal(1234, connection.Port);
        Assert.Equal("127.0.0.1", connection.IP.ToString());

        _ = connection.StartAsClient();
        await Task.Delay(TimeSpan.FromSeconds(1));

        Assert.False(connection.IsServer);
        Assert.True(connection.IsConnected());

        await Task.Delay(500);
        Assert.True(connection.IsConnected());

        var recvBuffer = connection.connections[0]!.ReciveBuffer;
        Assert.Equal(wpk.Size, recvBuffer.Remaining);

        var rpak = recvBuffer.ReadPacket();
        Assert.Equal(wpk.Size, rpak.Size);
        Assert.Equal(wpk.GetData(), rpak.Data);

        Assert.Equal(1, connection.HandledPackets);
    }

    [Fact]
    public async Task OnConnectedCalled()
    {
        
        TestClientConnection connection = new TestClientConnection(TestLogger, null);
        connection.Init("127.0.0.1", 1234);

        Assert.Equal(1234, connection.Port);
        Assert.Equal("127.0.0.1", connection.IP.ToString());

        _ = connection.StartAsClient();
        await Task.Delay(TimeSpan.FromSeconds(1));

        Assert.False(connection.IsServer);
        Assert.True(connection.IsConnected());

        Assert.Equal(2, connection.ConnectedCall);
    }

    [Fact]
    public async Task PacketHandle()
    {
        
        TestClientConnection connection = new TestClientConnection(TestLogger, null);
        connection.Init("127.0.0.1", 1234);

        Assert.Equal(1234, connection.Port);
        Assert.Equal("127.0.0.1", connection.IP.ToString());

        _ = connection.StartAsClient();
        await Task.Delay(TimeSpan.FromSeconds(1));

        Assert.False(connection.IsServer);
        Assert.True(connection.IsConnected());

        Assert.Equal(1, connection.HandledPackets);
    }

    [Fact]
    public async Task SendPacket()
    {
        
        TestClientConnection connection = new TestClientConnection(TestLogger, null);
        connection.Init("127.0.0.1", 1234);

        Assert.Equal(1234, connection.Port);
        Assert.Equal("127.0.0.1", connection.IP.ToString());

        _ = connection.StartAsClient();
        await Task.Delay(TimeSpan.FromSeconds(1));

        Assert.False(connection.IsServer);
        Assert.True(connection.IsConnected());

        IWPacket wpk = new WPacket();
        byte[] d = new byte[68];

        Random.Shared.NextBytes(d);
        wpk.WriteSeq(d);
        connection.Send(wpk, 0);

        var reciveBuffer = connection.connections[0]!.ReciveBuffer;
        var t = reciveBuffer.ReadPacket();

        Assert.Equal(wpk.Size, t.Size);
        Assert.Equal(wpk.GetData(), t.Data);
    }

    [Fact]
    public async Task Disconect()
    {
        TestClientConnection connection = new TestClientConnection(TestLogger, new DisconectConnectionFactory());
        connection.Init("127.0.0.1", 1234);

        Assert.Equal(1234, connection.Port);
        Assert.Equal("127.0.0.1", connection.IP.ToString());

        _ = connection.StartAsClient();
        await Task.Delay(TimeSpan.FromSeconds(1));

        Assert.False(connection.IsServer);
        Assert.True(connection.IsConnected());

        connection.Disconect(0);

        await Task.Delay(100);

        Assert.False(connection.IsConnected());
        Assert.Equal(1, connection.DisconnectedCall);
    }

    [Fact]
    public async Task SyncCall()
    {
        IWPacket wpk = new WPacket();
        byte[] d = new byte[68];
        Random.Shared.NextBytes(d);
        wpk.WriteSeq(d);



        
        IConnection connection = new TestClientConnection(TestLogger, null);
        connection.Init("127.0.0.1", 1234);

        Assert.Equal(1234, connection.Port);
        Assert.Equal("127.0.0.1", connection.IP.ToString());

        _ = connection.StartAsClient();
        await Task.Delay(TimeSpan.FromSeconds(1));

        Assert.False(connection.IsServer);
        Assert.True(connection.IsConnected());

        var response = await connection.SyncCall(wpk);
        Assert.NotNull(response);
        Assert.Equal(2147483648, response.GNACK);

        wpk.WriteGnack(response.GNACK);
        Assert.Equal(wpk.GetData(), response.Data);
    }

    [Fact]
    public async Task SyncFailCall()
    {
        IWPacket wpk = new WPacket();
        byte[] d = new byte[68];
        Random.Shared.NextBytes(d);
        wpk.WriteSeq(d);



        
        IConnection connection = new TestClientConnection(TestLogger, null);
        connection.Init("127.0.0.1", 1234);

        Assert.Equal(1234, connection.Port);
        Assert.Equal("127.0.0.1", connection.IP.ToString());

        _ = connection.StartAsClient();
        await Task.Delay(TimeSpan.FromSeconds(1));

        Assert.False(connection.IsServer);
        Assert.True(connection.IsConnected());

        var response = await connection.SyncCall(wpk);
        Assert.Null(response);
    }
}
