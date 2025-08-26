using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using TOP_Network.Enum;
using TOP_Network.Interfaces.Packets;
using TOP_Network.Packets;

namespace NetworkTest;

public class WritePacketTest
{
    [Fact]
    public void WriteByte()
    {
        IWPacket wpk = new WPacket();

        wpk.WriteChar(49);
        wpk.WriteChar(187);
        wpk.WriteChar(161);
        wpk.WriteChar(171);

        Assert.Equal([0x00, 0x0C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 49, 187, 161, 171], wpk.GetData());
    }

    [Fact]
    public void WriteShort()
    {
        IWPacket wpk = new WPacket();

        wpk.WriteShort(-10631);
        wpk.WriteShort(-2439);

        wpk.WriteShort(-27536);
        wpk.WriteShort(-14557);

        Assert.Equal([0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xD6, 0x79, 0xF6, 0x79, 0x94, 0x70, 0xC7, 0x23], wpk.GetData());
    }

    [Fact]
    public void WriteUShort()
    {
        IWPacket wpk = new WPacket();

        wpk.WriteUShort(54905);
        wpk.WriteUShort(63097);

        wpk.WriteUShort(38000);
        wpk.WriteUShort(50979);

        Assert.Equal([0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xD6, 0x79, 0xF6, 0x79, 0x94, 0x70, 0xC7, 0x23], wpk.GetData());
    }

    [Fact]
    public void WriteLong()
    {
        IWPacket wpk = new WPacket();

        wpk.WriteLong(-696650119);
        wpk.WriteLong(-1804548317);

        Assert.Equal([0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xD6, 0x79, 0xF6, 0x79, 0x94, 0x70, 0xC7, 0x23], wpk.GetData());
    }

    [Fact]
    public void WriteULong()
    {
        IWPacket wpk = new WPacket();

        wpk.WriteULong(3598317177);
        wpk.WriteULong(2490418979);

        Assert.Equal([0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xD6, 0x79, 0xF6, 0x79, 0x94, 0x70, 0xC7, 0x23], wpk.GetData());
    }

    [Fact]
    public void WriteLongLong()
    {
        IWPacket wpk = new WPacket();

        wpk.WriteLongLong(-2992089475369089245);

        Assert.Equal([0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xD6, 0x79, 0xF6, 0x79, 0x94, 0x70, 0xC7, 0x23], wpk.GetData());
    }

    [Fact]
    public void WriteULongLong()
    {
        IWPacket wpk = new WPacket();

        wpk.WriteULongLong(15454654598340462371);

        Assert.Equal([0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xD6, 0x79, 0xF6, 0x79, 0x94, 0x70, 0xC7, 0x23], wpk.GetData());
    }

    [Fact]
    public void WriteString()
    {
        IWPacket wpk = new WPacket();
        wpk.WriteString("This is a test String");

        Assert.Equal([0, 0x20,
            00, 00, 00, 00, 00, 00,
            0x00, 0x16,
            0x54, 0x68, 0x69, 0x73, 0x20, 0x69,
            0x73, 0x20, 0x61, 0x20, 0x74, 0x65, 0x73, 0x74, 0x20, 0x53, 0x74, 0x72, 0x69, 0x6E, 0x67, 0x00
        ], wpk.GetData());
    }

    [Fact]
    public void PacketConstroct()
    {
        IPacket packet = new Packet { LongSize = false };
        packet.Init([0x00, 0x08, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06]);
        HelperFunctions.HelperSize(packet, 8);

        IWPacket wpk = new WPacket(packet);
        Assert.Equal(8, wpk.Size);
        Assert.Equal(packet.Data, wpk.GetData());
    }

    [Fact]
    public void OverFlowWrite()
    {
        IWPacket wpk = new WPacket();
        var r = new byte[1024 * 64];
        Random.Shared.NextBytes(r);

        Assert.Throws<Exception>(() => wpk.WriteSeq(r));
    }

    [Fact]
    public void WriteSq()
    {
        IWPacket wpk = new WPacket();
        var r = new byte[25];
        Random.Shared.NextBytes(r);

        Assert.Equal(8, wpk.Size);

        wpk.WriteSeq(r);
        Assert.Equal(35, wpk.Size);
        Assert.Equal([0x00, 35, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 25, .. r], wpk.GetData());
    }
}
