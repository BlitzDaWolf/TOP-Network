using Microsoft.Extensions.Logging;
using TOP_Network;
using TOP_Network.Attributes;
using TOP_Network.Interfaces;
using TOP_Network.Interfaces.Packets;

namespace PacketConnectionTests.Abstractions;

[Server]
public class TestServerConnection : Connection
{
    public int ConnectedCall { get; private set; }
    public int DisconnectedCall { get; private set; }
    public int HandledPackets { get; private set; }

    public TestServerConnection(ILogger<Connection> logger, IConectionFactory conectionFactory, int maxClients = 2)
        : base(logger, conectionFactory, maxClients) { }


    public override Task OnConnected()
    {
        ConnectedCall++;
        return base.OnConnected();
    }

    public override Task OnConnected(int socket)
    {
        ConnectedCall++;
        return base.OnConnected(socket);
    }

    public override Task OnDisconect(int socket)
    {
        lock (this)
        {
            DisconnectedCall++;
        }
        return base.OnDisconect(socket);
    }

    public override Task<IPacket?> OnHandelPacket(IRPacket packet, int connection)
    {
        return base.OnHandelPacket(packet, connection);
    }
}
