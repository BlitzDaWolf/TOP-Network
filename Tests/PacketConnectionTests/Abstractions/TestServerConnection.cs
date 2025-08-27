using Microsoft.Extensions.Logging;
using TOP_Network;
using TOP_Network.Interfaces;

namespace PacketConnectionTests.Abstractions;

public class TestServerConnection : Connection, IConnection
{
    public TestServerConnection(ILogger<Connection> logger, IConectionFactory conectionFactory, int maxClients = 2)
        : base(logger, conectionFactory, maxClients) { }
}
