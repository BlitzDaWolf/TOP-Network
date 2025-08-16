using System.Reflection.PortableExecutable;
using TOP_Network.Attributes;
using TOP_Network.Extention;

namespace TOP_Network.Packets;

public class WPacket : Packet
{
    public WPacket() : base(new byte[32_768])
    {
        GetStream().Position = StartSize + 6;
    }
    public WPacket(Packet pk) : this()
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

    public override Packet Clone()
    {
        return base.Clone();
    }
}

public class RPacket : Packet
{
    int reversePoint = 0;

    public RPacket(byte[] data) : base(data)
    {
        GetStream().Position = StartSize + 6;
    }

    public byte ReadChar() => GetBitReader().ReadByte();

    public int ReadLong() => GetBitReader().ReadType<int>();
    public virtual byte[] ReadSeq() => GetBitReader().ReadType<byte[]>() ?? new byte[0];

    public short ReadShort() => GetBitReader().ReadType<short>();

    public virtual string ReadString() => (GetBitReader().ReadType<string>() ?? "").Replace("\0", "");

    public override void RemoveLast(int size)
    {
        base.RemoveLast(size);
        reversePoint -= size;
        reversePoint = Math.Max(reversePoint, 0);
    }

    public short ReverseReadShort()
    {
        var reader = GetBitReader();
        var currpos = reader.BaseStream.Position;
        reader.BaseStream.Position = Size - reversePoint - 2;

        var v = ReadShort();
        reversePoint += 2;

        reader.BaseStream.Position = currpos;
        return v;
    }

    public int ReverseReadLong()
    {
        var reader = GetBitReader();
        var currpos = reader.BaseStream.Position;
        reader.BaseStream.Position = Size - reversePoint - 4;

        var v = ReadLong();
        reversePoint += 4;

        reader.BaseStream.Position = currpos;
        return v;
    }

    public byte ReverseReadChar()
    {
        var reader = GetBitReader();
        var currpos = reader.BaseStream.Position;
        reader.BaseStream.Position = Size - reversePoint - 1;

        var v = ReadChar();
        reversePoint += 1;

        reader.BaseStream.Position = currpos;
        return v;
    }

    public void Remove(int v)
    {
        var reader = GetBitWriter();
        var remaining = Size - (int)reader.BaseStream.Position - v;
        var add = Data.Skip((int)reader.BaseStream.Position + 4).Take(remaining).Reverse().ToArray();
        reader.WriteBytes(add);
        WriteSize((int)reader.BaseStream.Position);
        GetBitWriter().BaseStream.Position = Size - remaining;
    }

    public string ReverseReadString()
    {
        var size = ReverseReadShort();
        var reader = GetBitReader();
        var currpos = reader.BaseStream.Position;
        reader.BaseStream.Position = Size - reversePoint - size - 2;

        var v = ReadString();
        reversePoint += size + 2;

        reader.BaseStream.Position = currpos;
        return v;
    }

    public void UnRead(int v)
    {
        GetBitWriter().BaseStream.Position = GetBitWriter().BaseStream.Position - v;
    }
}