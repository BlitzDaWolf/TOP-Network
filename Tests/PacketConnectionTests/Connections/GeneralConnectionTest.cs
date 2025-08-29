using System;
using Microsoft.Extensions.Logging;
using Moq;
using PacketConnectionTests.Abstractions;
using PacketConnectionTests.Abstractions.Facotries;
using TOP_Network.Interfaces;
using TOP_Network.Interfaces.Packets;

namespace PacketConnectionTests.Connections;

public class GeneralConnectionTest
{
    private readonly ILogger<TestGeneralConnection> TestLogger;

    public GeneralConnectionTest()
    {
        TestLogger = LoggerFactory.Create(conf => conf.ClearProviders()).CreateLogger<TestGeneralConnection>();
    }

    [Fact]
    public async Task SinglePacket()
    {
        PacketConnectionFactory factory = new PacketConnectionFactory();

        var mockConnection = new Mock<TestGeneralConnection>(TestLogger, factory);

        mockConnection.Setup(x => x.OnPreHandel(It.IsAny<IRPacket>(), It.IsAny<IMethodBag>()))
            .CallBase();
        mockConnection.Setup(x => x.OnHandelPacket(It.IsAny<IRPacket>(), It.IsAny<int>()))
            .CallBase();

        TestGeneralConnection conenction = mockConnection.Object;

        conenction.Init("127.0.0.1", 1234);

        await factory.Next();
        await Task.Delay(TimeSpan.FromSeconds(1));

        factory.Default.ReciveBuffer.AddData([
            0x00, 0x0C,
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x06, 0x00, 0x00, 0x00, 0x00,
        ]);
        await factory.Default.Next();
        await Task.Delay(TimeSpan.FromSeconds(0.2));

        mockConnection.Verify(x => x.HandleSinglePacket(It.IsAny<IRPacket>()), Times.AtLeastOnce());
    }

    [Fact]
    public async Task ArgumentPacket()
    {
        PacketConnectionFactory factory = new PacketConnectionFactory();

        var mockConnection = new Mock<TestGeneralConnection>(TestLogger, factory) { CallBase = true };
        TestGeneralConnection conenction = mockConnection.Object;

        conenction.Init("127.0.0.1", 1234);

        factory.Default.ReciveBuffer.AddData([
            0x00, 0x0C,
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x07, 0x00, 0x00, 0x00, 0x00,
        ]);
        await factory.Default.Next();
        await Task.Delay(TimeSpan.FromSeconds(1));

        mockConnection.Verify(x => x.HandleWithArgumentsPacket(It.IsAny<IRPacket>(), It.IsAny<int>()), Times.AtLeastOnce());
    }
    
    [Fact]
    public async Task DoubleArgumentPacket()
    {
        PacketConnectionFactory factory = new PacketConnectionFactory();

        var mockConnection = new Mock<TestGeneralConnection>(TestLogger, factory) { CallBase = true };
        TestGeneralConnection conenction = mockConnection.Object;

        conenction.Init("127.0.0.1", 1234);

        factory.Default.ReciveBuffer.AddData([
            0x00, 0x0C,
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x08, 0x00, 0x00, 0x00, 0x00,
        ]);
        await factory.Default.Next();
        await Task.Delay(TimeSpan.FromSeconds(1));

        mockConnection.Verify(x => x.HandleWithDoubleArgumentsPacket(It.IsAny<IRPacket>(), 9, "test"), Times.AtLeastOnce());
    }
}
