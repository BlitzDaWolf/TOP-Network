using Microsoft.Extensions.Logging;
using PacketConnectionTests.Abstractions;
using PacketConnectionTests.Abstractions.Facotries;
using TOP_Network;
using TOP_Network.Enum;
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
        connection.Start();

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
        TestClientConnection connection = new TestClientConnection(TestLogger, new NormalConnectionFactory { timing = 1 });
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
    public async Task OnConnectedCalled()
    {
        NormalConnectionFactory factory = new NormalConnectionFactory();
        TestClientConnection connection = new TestClientConnection(TestLogger, factory);
        connection.Init("127.0.0.1", 1234);

        Assert.Equal(1234, connection.Port);
        Assert.Equal("127.0.0.1", connection.IP.ToString());

        _ = connection.StartAsClient();
        await factory.Next();
        await Task.Delay(TimeSpan.FromSeconds(1));

        Assert.False(connection.IsServer);
        Assert.True(connection.IsConnected());

        Assert.Equal(2, connection.ConnectedCall);
    }

    [Fact]
    public async Task PacketHandle()
    {
        PacketConnectionFactory factory = new PacketConnectionFactory();
        TestClientConnection connection = new TestClientConnection(TestLogger, factory);
        connection.Init("127.0.0.1", 1234);

        Assert.Equal(1234, connection.Port);
        Assert.Equal("127.0.0.1", connection.IP.ToString());

        _ = connection.StartAsClient();
        await factory.Next();
        await Task.Delay(TimeSpan.FromSeconds(1));

        Assert.False(connection.IsServer);
        Assert.True(connection.IsConnected());

        factory.Default.ReciveBuffer.AddData([
            0x00, 0x0C,
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x06, 0x00, 0x00, 0x00, 0x00,
        ]);
        Assert.Equal(0, connection.HandledPackets);
        await factory.Default.Next();
        await Task.Delay(TimeSpan.FromSeconds(0.2));
        Assert.Equal(1, connection.HandledPackets);
    }

    [Fact]
    public async Task SendPacket()
    {
        PacketConnectionFactory factory = new PacketConnectionFactory();
        TestClientConnection connection = new TestClientConnection(TestLogger, factory);
        connection.Init("127.0.0.1", 1234);

        Assert.Equal(1234, connection.Port);
        Assert.Equal("127.0.0.1", connection.IP.ToString());

        _ = connection.StartAsClient();
        await factory.Next();
        await Task.Delay(TimeSpan.FromSeconds(1));

        Assert.False(connection.IsServer);
        Assert.True(connection.IsConnected());

        IWPacket wpk = new WPacket();
        byte[] d = new byte[68];

        Random.Shared.NextBytes(d);
        wpk.WriteSeq(d);
        connection.Send(wpk, 0);

        Assert.Equal(wpk.Size, factory.Default.SendBuffer.Remaining);
        Assert.False(factory.Default.SendBuffer.EOF);

        var t = factory.Default.SendBuffer.ReadPacket();

        Assert.Equal(wpk.Size, t.Size);
        Assert.Equal(wpk.GetData(), t.Data);
        Assert.True(factory.Default.SendBuffer.EOF);
        Assert.Equal(0, factory.Default.SendBuffer.Remaining);
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

        PacketConnectionFactory factory = new PacketConnectionFactory();
        TestClientConnection connection = new TestClientConnection(TestLogger, factory);
        connection.Init("127.0.0.1", 1234);

        Assert.Equal(1234, connection.Port);
        Assert.Equal("127.0.0.1", connection.IP.ToString());

        _ = connection.StartAsClient();
        await factory.Next();
        await Task.Delay(TimeSpan.FromSeconds(1));

        Assert.False(connection.IsServer);
        Assert.True(connection.IsConnected());

        var response = connection.SyncCall(wpk);
        var t = wpk.Clone<Packet>();
        t.WriteGnack(2147483649);
        factory.Default.ReciveBuffer.AddData(t);
        await factory.Default.Next();

        Assert.NotNull(response.Result);
        Assert.Equal(2147483649, response.Result.GNACK);

        wpk.WriteGnack(response.Result.GNACK);
        Assert.Equal(wpk.GetData(), response.Result.Data);
    }

    [Fact]
    public async Task SyncFailCall()
    {
        IWPacket wpk = new WPacket();
        byte[] d = new byte[68];
        Random.Shared.NextBytes(d);
        wpk.WriteSeq(d);

        PacketConnectionFactory factory = new PacketConnectionFactory();
        TestClientConnection connection = new TestClientConnection(TestLogger, factory);
        connection.Init("127.0.0.1", 1234);

        Assert.Equal(1234, connection.Port);
        Assert.Equal("127.0.0.1", connection.IP.ToString());

        _ = connection.StartAsClient();
        await Task.Delay(TimeSpan.FromSeconds(1));

        Assert.False(connection.IsServer);
        Assert.True(connection.IsConnected());

        var response = connection.SyncCall(wpk, 2500);
        Assert.Contains(2147483649, connection.Calls);
        await Task.Delay(2500);
        Assert.Null(response.Result);
    }

    [Fact]
    public async Task ReplayPacket()
    {
        PacketConnectionFactory factory = new PacketConnectionFactory();
        TestClientConnection connection = new TestClientConnection(TestLogger, factory);
        connection.Init("127.0.0.1", 1234);

        Assert.Equal(1234, connection.Port);
        Assert.Equal("127.0.0.1", connection.IP.ToString());

        _ = connection.StartAsClient();
        await factory.Next();
        await Task.Delay(TimeSpan.FromSeconds(1));

        IWPacket testPacket = new WPacket();
        testPacket.WriteCommand(TOP_Network.Enum.Commands.CMD_CM_PING);
        factory.Default.ReciveBuffer.AddData(testPacket);

        await factory.Default.Next();
        await Task.Delay(100);

        Assert.False(factory.Default.SendBuffer.EOF);
        var p = factory.Default.SendBuffer.ReadPacket();
        Assert.True(factory.Default.SendBuffer.EOF);
        Assert.Equal(Commands.CMD_MC_PING, p.Command);
        Assert.Equal(50, p.ReadLong());
    }
}
