using System;
using TOP_Network.Interfaces.Packets;
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
        Packet pkt = new Packet();
        pkt.Init([0x00, 0x0A, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00]);
        Assert.NotNull(pkt);
        Assert.NotEmpty(pkt.Data);
        HelperFunctions.HelperSize(pkt, 10);

        var rpkt = pkt.Clone<RPacket>();
        Assert.NotNull(rpkt);
        Assert.NotEmpty(rpkt.Data);
        HelperFunctions.HelperSize(rpkt, 10);
    }
    [Fact]
    public void ReadSeq()
    {
        V1Packet.LongSize = false;
        RPacket pkt = new RPacket();
        pkt.Init([0x00, 0x0E, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x04, 0x15, 0xF8, 0x3D, 0x4E]);
        Assert.NotNull(pkt);
        Assert.NotEmpty(pkt.Data);
        HelperFunctions.HelperSize(pkt, 14);

        Assert.Equal([0x15, 0xF8, 0x3D, 0x4E], pkt.ReadSeq());
        pkt.Final();
    }

    [Fact]
    public void ReverseReadSeq()
    {
        V1Packet.LongSize = false;
        RPacket pkt = new RPacket();
        pkt.Init([0x00, 0x20, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06,
            0x00, 0x14,
            0x54, 0x68, 0x69, 0x73, 0x20, 0x69, 0x73, 0x20, 0x61, 0x20, 0x74, 0x65, 0x73, 0x74, 0x20, 0x74, 0x65, 0x78, 0x74, 0x00,
            0x00, 0x14
        ]);
        Assert.NotNull(pkt);
        Assert.NotEmpty(pkt.Data);
        HelperFunctions.HelperSize(pkt, 32);

        var str = pkt.ReverseReadSeq();
        Assert.NotNull(str);
        Assert.NotEmpty(str);
        Assert.Equal([0x54, 0x68, 0x69, 0x73, 0x20, 0x69, 0x73, 0x20, 0x61, 0x20, 0x74, 0x65, 0x73, 0x74, 0x20, 0x74, 0x65, 0x78, 0x74, 0x00], str);

        pkt.Final();
    }

    [Fact]
    public void ReadString()
    {
        V1Packet.LongSize = false;
        RPacket pkt = new RPacket();
        pkt.Init([0x00, 0x1E, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06,
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
        RPacket pkt = new RPacket();
        pkt.Init([0x00, 0x20, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06,
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
        RPacket pkt = new RPacket();
        pkt.Init([0x00, 0x0C, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06, 0x15, 0xF8, 0x3D, 0x4E]);
        Assert.NotNull(pkt);
        Assert.NotEmpty(pkt.Data);
        HelperFunctions.HelperSize(pkt, 12);

        Assert.Equal(5624, pkt.ReadShort());
        pkt.UnRead(2);
        Assert.Equal(5624, pkt.ReadShort());
        Assert.Equal(15694, pkt.ReadShort());
        pkt.Final();
    }

    [Fact]
    public void SameReader()
    {
        Packet pkt = new Packet();
        pkt.Init([0x00, 0x0A, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00]);
        Assert.NotNull(pkt);
        Assert.NotEmpty(pkt.Data);
        HelperFunctions.HelperSize(pkt, 10);

        var r1 = pkt.GetReader();
        Assert.NotNull(r1);
        Assert.Same(r1, pkt.GetReader());
    }

    [Fact]
    public void ReadByte()
    {
        IRPacket pkt = new RPacket();
        pkt.Init([0x00, 0x0C, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06, 49, 187, 161, 171]);
        HelperFunctions.HelperSize(pkt, 12);

        Assert.Equal(49, pkt.ReadChar());
        Assert.Equal(187, pkt.ReadChar());
        Assert.Equal(161, pkt.ReadChar());
        Assert.Equal(171, pkt.ReadChar());
    }

    [Fact]
    public void ReverseReadByte()
    {
        IRPacket pkt = new RPacket();
        pkt.Init([0x00, 0x0C, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06, 49, 187, 161, 171]);
        HelperFunctions.HelperSize(pkt, 12);

        Assert.Equal(171, pkt.ReverseReadChar());
        Assert.Equal(161, pkt.ReverseReadChar());
        Assert.Equal(187, pkt.ReverseReadChar());
        Assert.Equal(49, pkt.ReverseReadChar());
    }

    [Fact]
    public void ReadShort()
    {
        IRPacket pkt = new RPacket();
        pkt.Init([0x00, 0x10, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06,
            0xD6, 0x79,
            0xF6, 0x79,
            0x94, 0x70,
            0xC7, 0x23
        ]);
        HelperFunctions.HelperSize(pkt, 16);

        Assert.Equal(-10631, pkt.ReadShort());
        Assert.Equal(-2439, pkt.ReadShort());

        Assert.Equal(-27536, pkt.ReadShort());
        Assert.Equal(-14557, pkt.ReadShort());
    }

    [Fact]
    public void ReadUShort()
    {
        IRPacket pkt = new RPacket();
        pkt.Init([0x00, 0x10, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06,
            0xD6, 0x79,
            0xF6, 0x79,
            0x94, 0x70,
            0xC7, 0x23
        ]);
        HelperFunctions.HelperSize(pkt, 16);

        Assert.Equal(54905, pkt.ReadUShort());
        Assert.Equal(63097, pkt.ReadUShort());

        Assert.Equal(38000, pkt.ReadUShort());
        Assert.Equal(50979, pkt.ReadUShort());
    }

    [Fact]
    public void ReverseReadShort()
    {
        IRPacket pkt = new RPacket();
        pkt.Init([0x00, 0x10, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06,
            0xD6, 0x79,
            0xF6, 0x79,
            0x94, 0x70,
            0xC7, 0x23
        ]);
        HelperFunctions.HelperSize(pkt, 16);


        Assert.Equal(-14557, pkt.ReverseReadShort());
        Assert.Equal(-27536, pkt.ReverseReadShort());
        Assert.Equal(-2439, pkt.ReverseReadShort());
        Assert.Equal(-10631, pkt.ReverseReadShort());
    }

    [Fact]
    public void ReverseReadUShort()
    {
        IRPacket pkt = new RPacket();
        pkt.Init([0x00, 0x10, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06,
            0xD6, 0x79,
            0xF6, 0x79,
            0x94, 0x70,
            0xC7, 0x23
        ]);
        HelperFunctions.HelperSize(pkt, 16);

        Assert.Equal(50979, pkt.ReverseReadUShort());
        Assert.Equal(38000, pkt.ReverseReadUShort());
        Assert.Equal(63097, pkt.ReverseReadUShort());
        Assert.Equal(54905, pkt.ReverseReadUShort());
    }

    [Fact]
    public void ReadLong()
    {
        IRPacket pkt = new RPacket();
        pkt.Init([0x00, 0x10, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06,
            0xD6, 0x79, 0xF6, 0x79,
            0x94, 0x70, 0xC7, 0x23
        ]);
        HelperFunctions.HelperSize(pkt, 16);

        Assert.Equal(-696650119, pkt.ReadLong());
        Assert.Equal(-1804548317, pkt.ReadLong());
    }

    [Fact]
    public void ReadULong()
    {
        IRPacket pkt = new RPacket();
        pkt.Init([0x00, 0x10, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06,
            0xD6, 0x79, 0xF6, 0x79,
            0x94, 0x70, 0xC7, 0x23
        ]);
        HelperFunctions.HelperSize(pkt, 16);

        Assert.Equal(3598317177, pkt.ReadULong());
        Assert.Equal(2490418979, pkt.ReadULong());
    }

    [Fact]
    public void ReverseReadLong()
    {
        IRPacket pkt = new RPacket();
        pkt.Init([0x00, 0x10, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06,
            0xD6, 0x79, 0xF6, 0x79,
            0x94, 0x70, 0xC7, 0x23
        ]);
        HelperFunctions.HelperSize(pkt, 16);

        Assert.Equal(-1804548317, pkt.ReverseReadLong());
        Assert.Equal(-696650119, pkt.ReverseReadLong());
    }

    [Fact]
    public void ReverseReadULong()
    {
        IRPacket pkt = new RPacket();
        pkt.Init([0x00, 0x10, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06,
            0xD6, 0x79, 0xF6, 0x79,
            0x94, 0x70, 0xC7, 0x23
        ]);
        HelperFunctions.HelperSize(pkt, 16);

        Assert.Equal(2490418979, pkt.ReverseReadULong());
        Assert.Equal(3598317177, pkt.ReverseReadULong());
    }

    [Fact]
    public void ReadLongLong()
    {
        IRPacket pkt = new RPacket();
        pkt.Init([0x00, 0x10, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06,
            0xD6, 0x79, 0xF6, 0x79, 0x94, 0x70, 0xC7, 0x23
        ]);
        HelperFunctions.HelperSize(pkt, 16);

        Assert.Equal(-2992089475369089245, pkt.ReadLongLong());
    }

    [Fact]
    public void ReadULongLong()
    {
        IRPacket pkt = new RPacket();
        pkt.Init([0x00, 0x10, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06,
            0xD6, 0x79, 0xF6, 0x79, 0x94, 0x70, 0xC7, 0x23
        ]);
        HelperFunctions.HelperSize(pkt, 16);

        Assert.Equal(15454654598340462371, pkt.ReadULongLong());
    }

    [Fact]
    public void ReverseReadLongLong()
    {
        IRPacket pkt = new RPacket();
        pkt.Init([0x00, 0x12, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06,
            0x80, 0x80,
            0xD6, 0x79, 0xF6, 0x79, 0x94, 0x70, 0xC7, 0x23
        ]);
        HelperFunctions.HelperSize(pkt, 18);

        Assert.Equal(-2992089475369089245, pkt.ReverseReadLongLong());
    }

    [Fact]
    public void ReverseReadULongLong()
    {
        IRPacket pkt = new RPacket();
        pkt.Init([0x00, 0x12, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06,
            0x80, 0x80,
            0xD6, 0x79, 0xF6, 0x79, 0x94, 0x70, 0xC7, 0x23
        ]);
        HelperFunctions.HelperSize(pkt, 18);

        Assert.Equal(15454654598340462371, pkt.ReverseReadULongLong());
    }

    [Fact]
    public void RemoveLast()
    {
        IRPacket pkt = new RPacket();
        pkt.Init([0x00, 0x10, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06,
            0xD6, 0x79,
            0xF6, 0x79,
            0x94, 0x70,
            0xC7, 0x23
        ]);
        HelperFunctions.HelperSize(pkt, 16);

        Assert.Equal(-14557, pkt.ReverseReadShort());
        Assert.Equal(2, pkt.ReversePoint);
        pkt.RemoveLast(2);
        Assert.Equal(0, pkt.ReversePoint);
    }
}
