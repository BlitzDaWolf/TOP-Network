using System;
using Microsoft.Extensions.Logging;
using TOP_Network;
using TOP_Network.Interfaces;
using TOP_Network.Interfaces.Packets;

namespace PacketConnectionTests.Abstractions;

public class TestClientConnection : Connection, IConnection
{
    public int ConnectedCall { get; private set; }
    public int DisconnectedCall { get; private set; }
    public int HandledPackets { get; private set; }

    public TestClientConnection(ILogger<TestClientConnection> logger, IConectionFactory conectionFactory)
        : base(logger, conectionFactory, 1) { }

    public TestClientConnection()
    {

    }

    public override Task OnConnected()
    {
        ConnectedCall++;
        return base.OnConnected();
    }

    public override Task OnConnected(int socket)
    {
        Assert.Equal(0, socket);
        ConnectedCall++;
        return base.OnConnected(socket);
    }

    public override Task OnDisconect(int socket)
    {
        Assert.Equal(0, socket);
        DisconnectedCall++;
        return base.OnDisconect(socket);
    }

    public override Task<IPacket?> OnHandelPacket(IRPacket packet, int connection)
    {
        Assert.Equal(0, connection);
        return base.OnHandelPacket(packet, connection);
    }
}
