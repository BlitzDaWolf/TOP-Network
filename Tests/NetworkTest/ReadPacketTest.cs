using System;
using TOP_Network.Packets;

namespace NetworkTest;

public class ReadPacketTest
{
    public ReadPacketTest()
    {
        V1Packet.LongSize = false;
    }

    [Fact]
    public void GetRead()
    {
        V1Packet.LongSize = false;
        V1Packet pkt = new V1Packet([0x00, 0x0A, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00]);
        Assert.NotNull(pkt);
        Assert.NotEmpty(pkt.Data);
        HelperFunctions.HelperSize(pkt, 10);

        var rpkt = pkt.GetRPacket();
        Assert.NotNull(rpkt);
        Assert.NotEmpty(rpkt.Data);
        HelperFunctions.HelperSize(rpkt, 10);
    }

    [Fact]
    public void ReadChar()
    {
        V1Packet.LongSize = false;
        RPacket pkt = new RPacket([0x00, 0x0C, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06, 0x15, 0xF8, 0x3D, 0x4E]);
        Assert.NotNull(pkt);
        Assert.NotEmpty(pkt.Data);
        HelperFunctions.HelperSize(pkt, 12);

        Assert.Equal(21, pkt.ReadChar());
        Assert.Equal(248, pkt.ReadChar());
        Assert.Equal(61, pkt.ReadChar());
        Assert.Equal(78, pkt.ReadChar());
        pkt.Final();
    }

    [Fact]
    public void ReadShort()
    {
        V1Packet.LongSize = false;
        RPacket pkt = new RPacket([0x00, 0x0C, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06, 0x15, 0xF8, 0x3D, 0x4E]);
        Assert.NotNull(pkt);
        Assert.NotEmpty(pkt.Data);
        HelperFunctions.HelperSize(pkt, 12);

        Assert.Equal(5624, pkt.ReadShort());
        Assert.Equal(15694, pkt.ReadShort());
        pkt.Final();
    }

    [Fact]
    public void ReadLong()
    {
        V1Packet.LongSize = false;
        RPacket pkt = new RPacket([0x00, 0x0C, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06, 0x15, 0xF8, 0x3D, 0x4E]);
        Assert.NotNull(pkt);
        Assert.NotEmpty(pkt.Data);
        HelperFunctions.HelperSize(pkt, 12);

        Assert.Equal(368590158, pkt.ReadLong());
        pkt.Final();
    }

    [Fact]
    public void ReadSeq()
    {
        V1Packet.LongSize = false;
        RPacket pkt = new RPacket([0x00, 0x0E, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x04, 0x15, 0xF8, 0x3D, 0x4E]);
        Assert.NotNull(pkt);
        Assert.NotEmpty(pkt.Data);
        HelperFunctions.HelperSize(pkt, 14);

        Assert.Equal([0x15, 0xF8, 0x3D, 0x4E], pkt.ReadSeq());
        pkt.Final();
    }

    [Fact]
    public void ReverseReadChar()
    {
        V1Packet.LongSize = false;
        RPacket pkt = new RPacket([0x00, 0x0E, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x04, 0x15, 0xF8, 0x3D, 0x4E]);
        Assert.NotNull(pkt);
        Assert.NotEmpty(pkt.Data);
        HelperFunctions.HelperSize(pkt, 14);

        Assert.Equal(78, pkt.ReverseReadChar());
        Assert.Equal(61, pkt.ReverseReadChar());
        Assert.Equal(248, pkt.ReverseReadChar());
        Assert.Equal(21, pkt.ReverseReadChar());
        pkt.Final();
    }

    [Fact]
    public void ReverseReadShort()
    {
        V1Packet.LongSize = false;
        RPacket pkt = new RPacket([0x00, 0x0E, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x04, 0x15, 0xF8, 0x3D, 0x4E]);
        Assert.NotNull(pkt);
        Assert.NotEmpty(pkt.Data);
        HelperFunctions.HelperSize(pkt, 14);

        Assert.Equal(15694, pkt.ReverseReadShort());
        Assert.Equal(5624, pkt.ReverseReadShort());
        pkt.Final();
    }

    [Fact]
    public void ReverseReadLong()
    {
        V1Packet.LongSize = false;
        RPacket pkt = new RPacket([0x00, 0x0E, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x04, 0x15, 0xF8, 0x3D, 0x4E]);
        Assert.NotNull(pkt);
        Assert.NotEmpty(pkt.Data);
        HelperFunctions.HelperSize(pkt, 14);

        Assert.Equal(368590158, pkt.ReverseReadLong());
        pkt.Final();
    }

    [Fact]
    public void RemoveLast()
    {
        V1Packet.LongSize = false;
        RPacket pkt = new RPacket([0x00, 0x0E, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x04, 0x15, 0xF8, 0x3D, 0x4E]);
        Assert.NotNull(pkt);
        Assert.NotEmpty(pkt.Data);
        HelperFunctions.HelperSize(pkt, 14);

        Assert.Equal(15694, pkt.ReverseReadShort());

        pkt.RemoveLast(2);
        HelperFunctions.HelperSize(pkt, 12);
        pkt.Final();
    }

    [Fact]
    public void ReadString()
    {
        V1Packet.LongSize = false;
        RPacket pkt = new RPacket([0x00, 0x1E, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06,
            0x00, 0x14,
            0x54, 0x68, 0x69, 0x73, 0x20, 0x69, 0x73, 0x20, 0x61, 0x20, 0x74, 0x65, 0x73, 0x74, 0x20, 0x74, 0x65, 0x78, 0x74, 0x00
        ]);
        Assert.NotNull(pkt);
        Assert.NotEmpty(pkt.Data);
        HelperFunctions.HelperSize(pkt, 30);

        var str = pkt.ReadString();
        Assert.NotNull(str);
        Assert.NotEmpty(str);
        Assert.Equal("This is a test text", str);

        pkt.Final();
    }

    [Fact]
    public void ReverseReadString()
    {
        V1Packet.LongSize = false;
        RPacket pkt = new RPacket([0x00, 0x20, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06,
            0x00, 0x14,
            0x54, 0x68, 0x69, 0x73, 0x20, 0x69, 0x73, 0x20, 0x61, 0x20, 0x74, 0x65, 0x73, 0x74, 0x20, 0x74, 0x65, 0x78, 0x74, 0x00,
            0x00, 0x14
        ]);
        Assert.NotNull(pkt);
        Assert.NotEmpty(pkt.Data);
        HelperFunctions.HelperSize(pkt, 32);

        var str = pkt.ReverseReadString();
        Assert.NotNull(str);
        Assert.NotEmpty(str);
        Assert.Equal("This is a test text", str);

        pkt.Final();
    }

    [Fact]
    public void UnRead()
    {
        V1Packet.LongSize = false;
        RPacket pkt = new RPacket([0x00, 0x0C, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06, 0x15, 0xF8, 0x3D, 0x4E]);
        Assert.NotNull(pkt);
        Assert.NotEmpty(pkt.Data);
        HelperFunctions.HelperSize(pkt, 12);

        Assert.Equal(5624, pkt.ReadShort());
        pkt.UnRead(2);
        Assert.Equal(5624, pkt.ReadShort());
        Assert.Equal(15694, pkt.ReadShort());
        pkt.Final();
    }
}
