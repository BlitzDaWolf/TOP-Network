using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PacketConnectionTests.Abstractions;
using PacketConnectionTests.Abstractions.Facotries;
using TOP_Network;
using TOP_Network.Exceptions;
using TOP_Network.Interfaces.Packets;
using TOP_Network.Packets;

namespace PacketConnectionTests.Connections;

public class ServerConnectionTest
{
    private readonly ILogger<TestServerConnection> TestLogger;

    public ServerConnectionTest()
    {
        TestLogger = LoggerFactory.Create(conf => conf.ClearProviders()).CreateLogger<TestServerConnection>();
    }

    [Fact]
    public async Task StartServerException()
    {
        IConnection connection = new TestServerConnection(TestLogger, new EmptyConnectionFactory());
        await Assert.ThrowsAsync<Exception>(() => connection.StartAsServer());
    }


    [Fact]
    public void InvalidInitException()
    {
        IConnection connection = new TestServerConnection(TestLogger, new EmptyConnectionFactory());
        Assert.Throws<InvalidPortInitException>(() => connection.Init("192.167.0.1", 0));
        Assert.Throws<InvalidIPInitException>(() => connection.Init("", 1234));
        Assert.Throws<FormatException>(() => connection.Init("123.345.567.789", 1234));
    }

    [Fact]
    public async Task StartServer()
    {
        
        IConnection connection = new TestServerConnection(TestLogger, new EmptyConnectionFactory());
        connection.Init("192.167.0.1", 789);

        Assert.Equal("192.167.0.1", connection.IP.ToString());
        Assert.Equal(789, connection.Port);

        _ = connection.StartAsServer();
        Assert.True(connection.IsServer);
    }

    [Fact]
    public async Task Connect2Clients()
    {
        NormalConnectionFactory factory = new NormalConnectionFactory();
        TestServerConnection connection = new TestServerConnection(TestLogger, factory);
        connection.Init("192.167.0.1", 789);

        Assert.Equal("192.167.0.1", connection.IP.ToString());
        Assert.Equal(789, connection.Port);

        _ = connection.StartAsServer();
        await Task.Delay(TimeSpan.FromSeconds(1));

        Assert.True(connection.IsServer);

        // Connect client 1
        await factory.Next();
        Assert.True(connection.IsConnected(0));
        Assert.False(connection.IsConnected(1));
        Assert.Equal(2, connection.ConnectedCall);
        // Connect client 2
        await factory.Next();
        Assert.True(connection.IsConnected(0));
        Assert.True(connection.IsConnected(1));
        Assert.Equal(4, connection.ConnectedCall);
    }

    [Fact]
    public async Task OverConnect()
    {
        NormalConnectionFactory factory = new NormalConnectionFactory();
        IConnection connection = new TestServerConnection(TestLogger, factory);
        connection.Init("192.167.0.1", 789);

        await connection.StartAsServer();
        await Task.Delay(TimeSpan.FromSeconds(1));

        // Connect client 1
        await factory.Next();
        // Connect client 2
        await factory.Next();
        // Connect client 3 and fail
        await factory.Next();

        Assert.True(connection.IsConnected(0));
        Assert.True(connection.IsConnected(1));

        Assert.Throws<IndexOutOfRangeException>(() => connection.IsConnected(2));
    }

    [Fact]
    public async Task SendToAll()
    {
        NormalConnectionFactory factory = new NormalConnectionFactory();
        TestServerConnection connection = new TestServerConnection(TestLogger, factory);
        connection.Init("192.167.0.1", 789);

        Assert.Equal("192.167.0.1", connection.IP.ToString());
        Assert.Equal(789, connection.Port);

        _ = connection.StartAsServer();
        await Task.Delay(TimeSpan.FromSeconds(1));

        Assert.True(connection.IsServer);

        // Connect client 1
        await factory.Next();
        Assert.True(connection.IsConnected(0));
        Assert.False(connection.IsConnected(1));
        Assert.Equal(2, connection.ConnectedCall);
        // Connect client 2
        await factory.Next();
        Assert.True(connection.IsConnected(0));
        Assert.True(connection.IsConnected(1));
        Assert.Equal(4, connection.ConnectedCall);

        IWPacket wpk = new WPacket();
        wpk.WriteString("Test");

        connection.SendToAll(wpk);

        ((NormalConnectionFactory.NormalNetowrkConnection)connection.connections[0]!).Next();
        ((NormalConnectionFactory.NormalNetowrkConnection)connection.connections[1]!).Next();

        var pkt1 = connection.connections[0]!.ReciveBuffer.ReadPacket();
        var pkt2 = connection.connections[1]!.ReciveBuffer.ReadPacket();

        Assert.Equal([pkt1.Size, pkt2.Size], [wpk.Size, wpk.Size]);
        Assert.Equal([pkt1.Data, pkt2.Data], [wpk.GetData(), wpk.GetData()]);
    }

    [Fact]
    public async Task DisconectAll()
    {
        DisconectConnectionFactory factory = new DisconectConnectionFactory();
        TestServerConnection connection = new TestServerConnection(TestLogger, factory);
        connection.Init("192.167.0.1", 789);

        _ = connection.StartAsServer();
        await Task.Delay(TimeSpan.FromSeconds(1));

        Assert.True(connection.IsServer);

        // Connect client 1
        await factory.Next();
        // Connect client 2
        await factory.Next();
        Assert.True(connection.IsConnected(0));
        Assert.True(connection.IsConnected(1));

        connection.DisconectAll();

        Assert.False(connection.IsConnected(0));
        Assert.False(connection.IsConnected(1));

        await Task.Delay(100);

        Assert.Equal(2, connection.DisconnectedCall);
    }
}
