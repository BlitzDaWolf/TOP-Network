using System;
using System.Runtime.CompilerServices;
using TOP_Network.Enum;
using TOP_Network.Packets;

namespace NetworkTest;

public class PacketTests
{
    public PacketTests()
    {
        Packet.LongSize = false;
    }


    [Fact]
    public void CreatePacket()
    {
        Packet pkt = new Packet();
        Assert.NotNull(pkt);
        Assert.Empty(pkt.Data);
    }

    /*[Fact]
    public void CheckLongSize()
    {
        Packet.LongSize = true;
        Assert.True(Packet.LongSize);
        Assert.Equal(4, Packet.StartSize);
    }*/

    [Fact]
    public void CheckShortSize()
    {
        Packet.LongSize = false;
        Assert.False(Packet.LongSize);
        Assert.Equal(2, Packet.StartSize);
    }

    [Fact]
    public void InitFunction()
    {
        Packet pkt = new Packet();
        Assert.NotNull(pkt);
        Assert.Empty(pkt.Data);
        pkt.Init([0x00, 0x08, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06]);
        Assert.NotEmpty(pkt.Data);
        HelperFunctions.HelperSize(pkt, 8);
    }

    /*[Fact]
    public void ConstructInitFunction()
    {
        Packet.LongSize = true;
        Packet pkt = new Packet([0x00, 0x00, 0x00, 0x0A, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06]);
        Assert.NotNull(pkt);
        Assert.NotEmpty(pkt.Data);
        HelperFunctions.HelperSize(pkt, 10);
    }*/

    [Fact]
    public void ValidGnack()
    {
        Packet pkt = new Packet([0x00, 0x08, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06]);
        Assert.NotNull(pkt);
        Assert.NotEmpty(pkt.Data);
        Assert.True(pkt.ValidGnack);
    }

    [Fact]
    public void InvalidGnack()
    {

        Packet pkt = new Packet([0x00, 0x08, 0x00, 0x00, 0x00, 0x01, 0x00, 0x06]);
        Assert.NotNull(pkt);
        Assert.NotEmpty(pkt.Data);
        Assert.False(pkt.ValidGnack);
    }

    [Fact]
    public void RandomGnack()
    {
        Packet pkt = new Packet([0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06]);

        var currentGnack = pkt.GetGnack();

        for (int i = 0; i < 10; i++)
        {
            pkt.AddRandomGnack();
            Assert.NotEqual(currentGnack, pkt.GetGnack());
        }
    }

    [Fact]
    public void RemoveData()
    {
        Packet pkt = new Packet([0x00, 0x0A, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00]);
        Assert.NotNull(pkt);
        Assert.NotEmpty(pkt.Data);
        HelperFunctions.HelperSize(pkt, 10);
        pkt.RemoveLast(2);
        HelperFunctions.HelperSize(pkt, 8);
    }

    /*[Fact]
    public void RemoveDataLong()
    {
        Packet.LongSize = true;
        Packet pkt = new Packet([0x00, 0x00, 0x00, 0x0C, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00]);
        Assert.NotNull(pkt);
        Assert.NotEmpty(pkt.Data);
        HelperFunctions.HelperSize(pkt, 12);
        pkt.RemoveLast(2);
        HelperFunctions.HelperSize(pkt, 10);
    }*/

    [Fact]
    public void DisplayHex()
    {
        Packet pkt = new Packet([0x00, 0x0A, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00]);
        Assert.NotNull(pkt);
        Assert.NotEmpty(pkt.Data);
        HelperFunctions.HelperSize(pkt, 10);
        var str = pkt.DisplayHex();
        Assert.NotEmpty(str);
        Assert.Equal("00 0A 80 00 00 00 00 06 00 00", str);
    }

    [Fact]
    public void ValidCommand()
    {
        Packet pkt = new Packet([0x00, 0x0A, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00]);
        Assert.NotNull(pkt);
        Assert.NotEmpty(pkt.Data);
        HelperFunctions.HelperSize(pkt, 10);
        Assert.Equal(Commands.CMD_CM_BEGINACTION, pkt.Command);
    }
}
