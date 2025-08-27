using System;
using Microsoft.Extensions.Logging;
using PacketConnectionTests.Abstractions;
using PacketConnectionTests.Abstractions.Streams;
using TOP_Network;
using TOP_Network.Exceptions;
using TOP_Network.Interfaces.Packets;
using TOP_Network.Packets;

namespace PacketConnectionTests;

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
        TestNetworkStream stream = new TestNetworkStream();
        IConnection connection = new TestServerConnection(TestLogger, new TestConenctionFactory<TestNetworkBuffer>(stream));
        await Assert.ThrowsAsync<Exception>(() => connection.StartAsServer());
    }


    [Fact]
    public void InvalidInitException()
    {
        TestNetworkStream stream = new TestNetworkStream();
        IConnection connection = new TestServerConnection(TestLogger, new TestConenctionFactory<TestNetworkBuffer>(stream));
        Assert.Throws<InvalidPortInitException>(() => connection.Init("192.167.0.1", 0));
        Assert.Throws<InvalidIPInitException>(() => connection.Init("", 1234));
        Assert.Throws<FormatException>(() => connection.Init("123.345.567.789", 1234));
    }

    [Fact]
    public async Task StartServer()
    {
        TestNetworkStream stream = new TestNetworkStream();
        IConnection connection = new TestServerConnection(TestLogger, new TestConenctionFactory<TestNetworkBuffer>(stream));
        connection.Init("192.167.0.1", 789);

        Assert.Equal("192.167.0.1", connection.IP.ToString());
        Assert.Equal(789, connection.Port);

        _ = connection.StartAsServer();
        await Task.Delay(TimeSpan.FromSeconds(1));

        Assert.True(connection.IsServer);
    }

    [Fact]
    public async Task Connect2Clients()
    {
        TestNetworkStream stream = new TestNetworkStream();
        IConnection connection = new TestServerConnection(TestLogger, new TestConenctionFactory<TestNetworkBuffer>(stream));
        connection.Init("192.167.0.1", 789);

        Assert.Equal("192.167.0.1", connection.IP.ToString());
        Assert.Equal(789, connection.Port);

        _ = connection.StartAsServer();
        await Task.Delay(TimeSpan.FromSeconds(1));

        Assert.True(connection.IsServer);

        // Connect client 1
        // Connect client 2

        Assert.True(connection.IsConnected(0));
        Assert.True(connection.IsConnected(1));
    }

    [Fact]
    public void OverConnect()
    {
        TestNetworkStream stream = new TestNetworkStream();
        IConnection connection = new TestServerConnection(TestLogger, new TestConenctionFactory<TestNetworkBuffer>(stream));
        connection.Init("192.167.0.1", 789);

        Assert.Equal("192.167.0.1", connection.IP.ToString());
        Assert.Equal(789, connection.Port);

        // Connect client 1
        // Connect client 2
        // Connect client 3 and fail

        Assert.True(connection.IsConnected(0));
        Assert.True(connection.IsConnected(1));

        Assert.Throws<Exception>(() => connection.IsConnected(2));
    }

    [Fact]
    public void SendToAll()
    {

        TestNetworkStream stream = new TestNetworkStream();
        IConnection connection = new TestServerConnection(TestLogger, new TestConenctionFactory<TestNetworkBuffer>(stream));
        connection.Init("192.167.0.1", 789);

        // Connect client 1
        // Connect client 2

        IWPacket wpk = new WPacket();
        wpk.WriteString("Test");

        connection.SendToAll(wpk);

        var pkt1 = connection.connections[0]!.SendBuffer.ReadPacket();
        var pkt2 = connection.connections[1]!.SendBuffer.ReadPacket();

        Assert.Equal([pkt1.Size, pkt2.Size], [wpk.Size, wpk.Size]);
        Assert.Equal([pkt1.Data, pkt2.Data], [wpk.GetData(), wpk.GetData()]);
    }

    [Fact]
    public void DisconectAll()
    {

        TestNetworkStream stream = new TestNetworkStream();
        IConnection connection = new TestServerConnection(TestLogger, new TestConenctionFactory<TestNetworkBuffer>(stream));
        connection.Init("192.167.0.1", 789);

        // Connect client 1
        // Connect client 2

        IWPacket wpk = new WPacket();
        wpk.WriteString("Test");

        connection.SendToAll(wpk);


        Assert.True(connection.IsConnected(0));
        Assert.True(connection.IsConnected(1));

        connection.DisconectAll();

        Assert.False(connection.IsConnected(0));
        Assert.False(connection.IsConnected(1));
    }
}
