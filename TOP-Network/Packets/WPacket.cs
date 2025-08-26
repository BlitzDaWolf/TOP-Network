using TOP_Network;
using TOP_Network.Extention;

namespace TOP_Network.Packets;

public class WPacket : V1Packet
{
    public WPacket() : base(new byte[32_768])
    {
        GetStream().Position = StartSize + 6;
        WriteSize(StartSize + 6);
    }
    public WPacket(V1Packet pk) : this()
    {
        GetStream().Position = 0;
        GetBitWriter().WriteBytes(pk.GetData().Reverse().ToArray());
    }

    public void WriteLong(int value)
    {
        base.GetBitWriter().WriteType(value);
        WriteSize((int)GetStream().Position);
    }
    public void WriteSeq(byte[] data)
    {
        base.GetBitWriter().WriteType(data);
        WriteSize((int)GetStream().Position);
    }

    public int WriteString(string value)
    {
        if (value.Length == 0) value += "\0";
        if (value.Last() != 0x00) value += '\0';
        base.GetBitWriter().WriteType(value);
        // WriteSeq(value.Select(x => (byte)x).ToArray());
        WriteSize((int)GetStream().Position);
        return value.Length;
    }

    public void WriteShort(short v)
    {
        GetBitWriter().WriteType(v);
        WriteSize((int)GetStream().Position);
    }

    public void WriteChar(byte v)
    {
        GetBitWriter().WriteType(v);
        WriteSize((int)GetStream().Position);
    }
}

