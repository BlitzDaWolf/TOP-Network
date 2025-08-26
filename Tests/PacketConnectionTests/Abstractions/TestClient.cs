using System;
using Microsoft.Extensions.Logging;
using TOP_Network;

namespace PacketConnectionTests.Abstractions;

public class TestClient : Connection
{
    public TestClient(ILogger<Connection> logger, int maxClients = 1) : base(logger, maxClients)
    {
    }
}
