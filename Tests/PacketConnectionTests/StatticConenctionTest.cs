using System;
using Microsoft.Extensions.Logging;
using Moq;
using TOP_Network;
using TOP_Network.Interfaces.Packets;

namespace PacketConnectionTests;

public class StatticConenctionTest
{
    private readonly Mock<ILogger<Connection>> MockLog;
    private readonly Mock<Connection> MockConnection;
    private readonly Mock<IRPacket> MockPacket;

    public StatticConenctionTest()
    {
        MockLog = new Mock<ILogger<Connection>>();
        MockConnection = new Mock<Connection>(MockLog.Object, null, 10);

        MockPacket = new Mock<IRPacket>();
    }

    [Fact]
    public void Init()
    {
        Connection<Connection>.SetInstance(MockConnection.Object);
        Assert.NotNull(Connection<Connection>.Instance);
    }
}
