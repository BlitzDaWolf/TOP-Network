using System;
using System.Runtime.CompilerServices;
using TOP_Network.Enum;
using TOP_Network.Packets;

namespace NetworkTest;

public class V1PacketTests
{
    public V1PacketTests()
    {
        V1Packet.LongSize = false;
    }


    [Fact]
    public void CreatePacket()
    {
        V1Packet pkt = new V1Packet();
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
        V1Packet.LongSize = false;
        Assert.False(V1Packet.LongSize);
        Assert.Equal(2, V1Packet.StartSize);
    }

    [Fact]
    public void InitFunction()
    {
        V1Packet pkt = new V1Packet();
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
        V1Packet pkt = new V1Packet([0x00, 0x08, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06]);
        Assert.NotNull(pkt);
        Assert.NotEmpty(pkt.Data);
        Assert.True(pkt.ValidGnack);
    }

    [Fact]
    public void InvalidGnack()
    {

        V1Packet pkt = new V1Packet([0x00, 0x08, 0x00, 0x00, 0x00, 0x01, 0x00, 0x06]);
        Assert.NotNull(pkt);
        Assert.NotEmpty(pkt.Data);
        Assert.False(pkt.ValidGnack);
    }

    [Fact]
    public void RandomGnack()
    {
        V1Packet pkt = new V1Packet([0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06]);

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
        V1Packet pkt = new V1Packet([0x00, 0x0A, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00]);
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
        V1Packet pkt = new V1Packet([0x00, 0x0A, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00]);
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
        V1Packet pkt = new V1Packet([0x00, 0x0A, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00]);
        Assert.NotNull(pkt);
        Assert.NotEmpty(pkt.Data);
        HelperFunctions.HelperSize(pkt, 10);
        Assert.Equal(Commands.CMD_CM_BEGINACTION, pkt.Command);
    }
}
