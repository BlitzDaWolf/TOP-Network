using System;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;
using TOP_Network;
using TOP_Network.Attributes;
using TOP_Network.Enum;
using TOP_Network.Interfaces;
using TOP_Network.Interfaces.Packets;

namespace PacketConnectionTests.Abstractions;

[Client(false)]
public class TestGeneralConnection : Connection<TestGeneralConnection>
{
    public TestGeneralConnection(ILogger<TestGeneralConnection> logger, IConectionFactory conectionFactory)
        : base(logger, conectionFactory, 1)
    {
    }

    public override void OnPreHandel(IRPacket packet, int connection, IMethodBag bag)
    {
        switch (packet.Command)
        {
            case Commands.CMD_CM_ENDACTION:
                bag.SetValue("testint", 5);
                break;
            case Commands.CMD_CM_SYNATTR:
                bag.SetValue("testint", 9);
                bag.SetValue("hallo", "test");
                break;
            default:
                base.OnPreHandel(packet, connection, bag);
                break;
        }

    }

    [PacketHandle(Commands.CMD_CM_BEGINACTION)]
    public virtual void HandleSinglePacket(IRPacket packet) { }


    [PacketHandle(Commands.CMD_CM_ENDACTION)]
    public virtual void HandleWithArgumentsPacket(IRPacket packet, int testInt)
    {
        Assert.Equal(5, testInt);
        Assert.NotNull(packet.Data);
    }

    [PacketHandle(Commands.CMD_CM_SYNATTR)]
    public virtual void HandleWithDoubleArgumentsPacket(IRPacket packet, int testInt, string hallo)
    {
        Assert.Equal(9, testInt);
        Assert.Equal("test", hallo);
        Assert.NotNull(packet.Data);
    }
}
