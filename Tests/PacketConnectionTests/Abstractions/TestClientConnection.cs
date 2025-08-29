using System;
using Microsoft.Extensions.Logging;
using TOP_Network;
using TOP_Network.Attributes;
using TOP_Network.Interfaces;
using TOP_Network.Interfaces.Packets;
using TOP_Network.Packets;

namespace PacketConnectionTests.Abstractions;

[Client(false)]
public class TestClientConnection : Connection
{
    public int ConnectedCall { get; private set; }
    public int DisconnectedCall { get; private set; }
    public int HandledPackets { get; private set; }

    public TestClientConnection(ILogger<TestClientConnection> logger, IConectionFactory conectionFactory)
        : base(logger, conectionFactory, 1) { }

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
        HandledPackets++;
        base.OnHandelPacket(packet, connection);

        if (packet.Command == TOP_Network.Enum.Commands.CMD_CM_PING)
        {
            IWPacket wpk = new WPacket();
            wpk.WriteCommand(TOP_Network.Enum.Commands.CMD_MC_PING);
            wpk.WriteLong(50);
            return Task.FromResult<IPacket?>(wpk);
        }
        return Task.FromResult<IPacket?>(null);
    }
}
