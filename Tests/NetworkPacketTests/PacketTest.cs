using System;
using TOP_Network.Enum;
using TOP_Network.Interfaces.Packets;
using TOP_Network.Packets;

namespace NetworkPacketTests;

public class PacketTest
{
    [Fact]
    public void PacketCreate()
    {
        IPacket packet = new Packet { LongSize = false };
        packet.Init([0x00, 0x08, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06]);
        HelperFunctions.HelperSize(packet, 8);
    }

    [Fact]
    public void PacketLongCreate()
    {
        IPacket packet = new Packet { LongSize = true };
        packet.Init([0x00, 0x00, 0x00, 0x0A, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06]);
        HelperFunctions.HelperSize(packet, 10);
    }

    [Fact]
    public void ValidClone()
    {
        IPacket packet = new Packet { LongSize = false };
        packet.Init([0x00, 0x08, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06]);
        HelperFunctions.HelperSize(packet, 8);
        IPacket Clone = packet.Clone<Packet>();
        HelperFunctions.HelperSize(Clone, 8);

        Assert.Equal(packet.Data, Clone.Data);
    }

    [Fact]
    public void Remove()
    {
        IPacket packet = new Packet { LongSize = false };
        packet.Init([0x00, 0x0E, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06, 0x85, 0x70, 255, 157, 12, 80]);
        HelperFunctions.HelperSize(packet, 14);
        packet.RemoveLast(2);
        HelperFunctions.HelperSize(packet, 12);
        packet.Remove(2);
        HelperFunctions.HelperSize(packet, 10);
    }

    [Fact]
    public void RemoveLong()
    {
        IPacket packet = new Packet { LongSize = true };
        packet.Init([0x00, 0x00, 0x00, 0x10, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06, 0x85, 0x70, 255, 157, 12, 80]);
        HelperFunctions.HelperSize(packet, 16);
        packet.RemoveLast(2);
        HelperFunctions.HelperSize(packet, 14);
    }

    [Fact]
    public void GetData()
    {
        IPacket packet = new Packet { LongSize = true };
        packet.Init([0x00, 0x00, 0x00, 0x0A, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06]);
        HelperFunctions.HelperSize(packet, 10);
        Assert.Equal(packet.Data, packet.GetData());
    }

    [Fact]
    public void ValidGnack()
    {
        IPacket packet = new Packet { LongSize = false };
        packet.Init([0x00, 0x08, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06]);
        HelperFunctions.HelperSize(packet, 8);
        Assert.True(packet.ValidGnack);
    }

    [Fact]
    public void InvalidGnack()
    {
        IPacket packet = new Packet { LongSize = false };
        packet.Init([0x00, 0x08, 0x08, 0x00, 0x00, 0x00, 0x00, 0x06]);
        HelperFunctions.HelperSize(packet, 8);
        Assert.False(packet.ValidGnack);
    }

    [Fact]
    public void CorrectCommand()
    {
        IPacket packet = new Packet { LongSize = false };
        packet.Init([0x00, 0x08, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06]);
        HelperFunctions.HelperSize(packet, 8);
        Assert.Equal(Commands.CMD_CM_BEGINACTION, packet.Command);
    }

    [Fact]
    public void WriteCommand()
    {
        IPacket packet = new Packet { LongSize = false };
        packet.Init([0x00, 0x08, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06]);
        HelperFunctions.HelperSize(packet, 8);
        Assert.Equal(Commands.CMD_CM_BEGINACTION, packet.Command);
        packet.WriteCommand(Commands.CMD_MC_CHAPSTR);
        Assert.Equal(Commands.CMD_MC_CHAPSTR, packet.Command);
        HelperFunctions.HelperSize(packet, 8);
    }

    [Fact]
    public void WriteGnack()
    {
        IPacket packet = new Packet { LongSize = false };
        packet.Init([0x00, 0x08, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06]);
        HelperFunctions.HelperSize(packet, 8);
        Assert.True(packet.ValidGnack);
        packet.WriteGnack((uint)234891354);
        Assert.False(packet.ValidGnack);
        HelperFunctions.HelperSize(packet, 8);
    }
}
