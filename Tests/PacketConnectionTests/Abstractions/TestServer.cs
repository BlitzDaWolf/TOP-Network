using System;
using Microsoft.Extensions.Logging;
using TOP_Network;

namespace PacketConnectionTests.Abstractions;

public class TestServer : Connection
{
    public TestServer(ILogger<Connection> logger, int maxClients = 10) : base(logger, maxClients)
    {
    }
}
