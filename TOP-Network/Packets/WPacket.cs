
using System.Runtime.CompilerServices;
using TOP_Network.Enum;
using TOP_Network.Extention;

namespace TOP_Network.Packets;

public class WPacket : Packet
{
    public WPacket() : base(new byte[4096])
    {
        GetStream().Position = StartSize + 6;
    }

    public void WriteLong(int value)
    {
        base.GetBitWriter().WriteType(value);
    }
    public void WriteSeq(byte[] data)
    {
        base.GetBitWriter().WriteType(data);
    }

    public void WriteString(string value)
    {
        if (value.Last() != 0x00) value += '\0';
        base.GetBitWriter().WriteType(value);
        // WriteSeq(value.Select(x => (byte)x).ToArray());
    }

    public void WriteCMD(Commands command)
    {
        using var writer = base.GetBitWriter();
        var current = writer.BaseStream.Position;
        writer.BaseStream.Position = 0;

        if (LongSize)
            writer.WriteType((short)command);
        else
            writer.WriteType((int)command);

        writer.BaseStream.Position = current;
    }

    public void WriteShort(short v)
    {
        GetBitWriter().WriteType(v);
    }

    public void WriteChar(byte v)
    {
        GetBitWriter().WriteType(v);
    }
}

public class RPacket : Packet
{
    public RPacket(byte[] data) : base(data)
    {
        GetStream().Position = StartSize + 6;
    }

    public int ReadLong() => GetBitReader().ReadType<int>();
    public byte[] ReadSeq() => GetBitReader().ReadType<byte[]>() ?? new byte[0];
    public string ReadString() => GetBitReader().ReadType<string>() ?? "";
}