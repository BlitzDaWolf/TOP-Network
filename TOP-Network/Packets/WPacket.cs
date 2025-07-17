using TOP_Network.Extention;

namespace TOP_Network.Packets;

public class WPacket : Packet
{
    public WPacket() : base(new byte[32_768])
    {
        GetStream().Position = StartSize + 6;
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

    public void WriteString(string value)
    {
        if (value.Last() != 0x00) value += '\0';
        base.GetBitWriter().WriteType(value);
        // WriteSeq(value.Select(x => (byte)x).ToArray());
        WriteSize((int)GetStream().Position);
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

    public override Packet Clone()
    {
        return base.Clone();
    }
}

public class RPacket : Packet
{
    public RPacket(byte[] data) : base(data)
    {
        GetStream().Position = StartSize + 6;
    }

    public byte ReadChar() => GetBitReader().ReadByte();

    public int ReadLong() => GetBitReader().ReadType<int>();
    public byte[] ReadSeq() => GetBitReader().ReadType<byte[]>() ?? new byte[0];

    public short ReadShort() => GetBitReader().ReadType<short>();

    public string ReadString() => (GetBitReader().ReadType<string>() ?? "").Replace("\0", "");
}