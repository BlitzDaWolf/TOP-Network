using System;
using TOP_Network;
using TOP_Network.Interfaces.Packets;
using TOP_Network.Packets;

namespace PacketConnectionTests;

public class BufferTest
{
    [Fact]
    public void AddData()
    {
        NetworkBuffer buffer = new NetworkBuffer();
        buffer.AddData([0x00, 0x01, 0x02, 0x03]);

        Assert.Equal(4, buffer.Remaining);
    }

    [Fact]
    public void AddPacket()
    {
        IWPacket wpk = new WPacket();
        Assert.Equal(8, wpk.Size);
        wpk.WriteString("test packet");

        NetworkBuffer buffer = new NetworkBuffer();
        buffer.AddData(wpk);
        Assert.Equal(wpk.Size, buffer.Remaining);
    }

    [Fact]
    public void Add2Packet()
    {
        IWPacket wpk1 = new WPacket();
        Assert.Equal(8, wpk1.Size);
        wpk1.WriteString("test packet1");
        IWPacket wpk2 = new WPacket();
        Assert.Equal(8, wpk2.Size);
        wpk2.WriteString("test packet 2 other size");

        NetworkBuffer buffer = new NetworkBuffer();
        buffer.AddData(wpk1);
        buffer.AddData(wpk2);
        Assert.Equal(wpk1.Size + wpk2.Size, buffer.Remaining);
    }

    [Fact]
    public void IsEOF()
    {
        NetworkBuffer buffer = new NetworkBuffer();
        Assert.True(buffer.EOF);
        buffer.AddData([0x00, 0x01, 0x02, 0x03]);
        Assert.False(buffer.EOF);
    }

    [Fact]
    public void Step()
    {
        NetworkBuffer buffer = new NetworkBuffer();
        buffer.AddData([0x00, 0x01, 0x02, 0x03]);
        Assert.Equal(4, buffer.Remaining);
        buffer.ReadBuffer(2);
        Assert.Equal(2, buffer.Remaining);
        buffer.SafeStep();
    }

    [Fact]
    public void ReadBuffer()
    {
        NetworkBuffer buffer = new NetworkBuffer();
        buffer.AddData([0x00, 0x01, 0x02, 0x03]);
        Assert.Equal(4, buffer.Remaining);
        Assert.False(buffer.EOF);
        var result = buffer.ReadBuffer(2);
        Assert.Equal(2, buffer.Remaining);
        Assert.Equal([0x00, 0x01], result);
        Assert.False(buffer.EOF);
    }

    [Fact]
    public void ReadAllBuffer()
    {
        NetworkBuffer buffer = new NetworkBuffer();
        buffer.AddData([0x00, 0x01, 0x02, 0x03]);
        Assert.Equal(4, buffer.Remaining);
        Assert.False(buffer.EOF);
        var result = buffer.ReadAll();
        Assert.Equal(0, buffer.Remaining);
        Assert.Equal([0x00, 0x01, 0x02, 0x03], result);
        Assert.True(buffer.EOF);
    }

    [Fact]
    public void Peek()
    {
        NetworkBuffer buffer = new NetworkBuffer();
        buffer.AddData([0x00, 0x01, 0x02, 0x03]);
        Assert.Equal(4, buffer.Remaining);
        Assert.False(buffer.EOF);
        buffer.ReadBuffer(1);
        var result = buffer.Peek(2);
        Assert.Equal(3, buffer.Remaining);
        Assert.Equal([0x01, 0x02], result);
        Assert.False(buffer.EOF);
    }

    [Fact]
    public void ReadPacket()
    {
        IWPacket wpk = new WPacket();
        Assert.Equal(8, wpk.Size);
        wpk.WriteString("test packet");

        NetworkBuffer buffer = new NetworkBuffer();
        buffer.AddData(wpk);
        Assert.Equal(wpk.Size, buffer.Remaining);

        IRPacket result = buffer.ReadPacket();
        Assert.Equal(wpk.Size, result.Size);
        Assert.True(buffer.EOF);
        Assert.Equal(wpk.GetData(), result.GetData());
    }

    [Fact]
    public void ReadPacketOverFlow()
    {
        IWPacket wpk = new WPacket();
        Assert.Equal(8, wpk.Size);
        wpk.WriteString("test packet");

        NetworkBuffer buffer = new NetworkBuffer();
        buffer.AddData([0xFF]);
        buffer.AddData(wpk);
        Assert.Equal(wpk.Size + 1, buffer.Remaining);

        Assert.Throws<Exception>(() => buffer.ReadPacket());
    }

    [Fact]
    public void InsufficientBytes()
    {
        NetworkBuffer buffer = new NetworkBuffer();
        buffer.AddData([0xFF]);
        Assert.Equal(1, buffer.Remaining);

        Assert.Throws<Exception>(() => buffer.ReadPacket());
    }

    [Fact]
    public void RequestTooManyBytes()
    {
        NetworkBuffer buffer = new NetworkBuffer();
        buffer.AddData([0xFF]);
        Assert.Throws<Exception>(() => buffer.ReadBuffer(2));
    }
}
