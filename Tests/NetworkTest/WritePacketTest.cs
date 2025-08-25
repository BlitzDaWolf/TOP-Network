using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using TOP_Network.Enum;
using TOP_Network.Packets;

namespace NetworkTest;

public class WritePacketTest
{
    public WritePacketTest()
    {
        Packet.LongSize = false;
    }

    [Fact]
    public void Constructor()
    {
        Packet.LongSize = false;
        WPacket wpk = new WPacket();
        Assert.NotEmpty(wpk.Data);
        Assert.Equal(32_768, wpk.Data.Length);
        Assert.Equal(8, wpk.Size);
    }

    [Fact]
    public void ConstructorWithPacket()
    {
        Packet.LongSize = false;
        Packet pkt = new Packet([0x00, 0x0A, 0x80, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00]);
        Assert.NotNull(pkt);
        Assert.NotEmpty(pkt.Data);
        HelperFunctions.HelperSize(pkt, 10);

        WPacket wpk = new WPacket(pkt);
        Assert.Equal(10, pkt.Size);
    }

    [Fact]
    public void WriteLong()
    {
        Packet.LongSize = false;
        WPacket wpk = new WPacket();
        Assert.NotEmpty(wpk.Data);

        wpk.WriteLong(5080);
        Assert.Equal(Packet.StartSize + 6 + 4, wpk.Size);
    }

    [Fact]
    public void WriteShort()
    {
        Packet.LongSize = false;
        WPacket wpk = new WPacket();
        Assert.NotEmpty(wpk.Data);

        wpk.WriteShort(5080);
        Assert.Equal(Packet.StartSize + 6 + 2, wpk.Size);
    }

    [Fact]
    public void WriteChar()
    {
        Packet.LongSize = false;
        WPacket wpk = new WPacket();
        Assert.NotEmpty(wpk.Data);

        wpk.WriteChar(128);
        Assert.Equal(Packet.StartSize + 6 + 1, wpk.Size);
    }

    [Fact]
    public void WriteSq()
    {
        WPacket wpk = new WPacket();
        Assert.NotEmpty(wpk.Data);

        wpk.WriteSeq([0x00, 0x01, 0x02, 0x04, 0x08, 0x10]);
        Assert.Equal(Packet.StartSize + 6 + 8, wpk.Size);
    }

    [Fact]
    public void WriteEmptyString()
    {
        WPacket wpk = new WPacket();
        Assert.NotEmpty(wpk.Data);

        wpk.WriteString("");
        Assert.Equal(Packet.StartSize + 6 + 3, wpk.Size);
    }

    [Fact]
    public void WriteNonTeminateString()
    {
        WPacket wpk = new WPacket();
        Assert.NotEmpty(wpk.Data);

        wpk.WriteString("test");
        Assert.Equal(Packet.StartSize + 6 + 7, wpk.Size);
    }

    [Fact]
    public void WriteTeminateString()
    {
        WPacket wpk = new WPacket();
        Assert.NotEmpty(wpk.Data);

        wpk.WriteString("test\0");
        Assert.Equal(Packet.StartSize + 6 + 7, wpk.Size);
    }

    [Fact]
    public void WriteCMD()
    {
        WPacket wpk = new WPacket();
        Assert.NotEmpty(wpk.Data);

        wpk.WriteCMD(TOP_Network.Enum.Commands.CMD_CM_SAY);
        Assert.Equal(Commands.CMD_CM_SAY, wpk.Command);
    }
}
