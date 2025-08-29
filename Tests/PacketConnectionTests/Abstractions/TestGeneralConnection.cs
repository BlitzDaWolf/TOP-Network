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
public class TestGeneralConnection : Connection
{
    public TestGeneralConnection(ILogger<Connection> logger, IConectionFactory conectionFactory)
        : base(logger, conectionFactory, 1)
    {
    }

    public override void OnPreHandel(IRPacket packet, IMethodBag Bag)
    {
        switch (packet.Command)
        {
            case Commands.CMD_CM_ENDACTION:
                Bag.SetValue("testint", 5);
                break;
            case Commands.CMD_CM_SYNATTR:
                Bag.SetValue("testint", 9);
                Bag.SetValue("hallo", "test");
                break;
            default:
                base.OnPreHandel(packet, Bag);
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
