using System;
using TOP_Network.Extention;

namespace TOP_Network.Packets;

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

    // Obsolete
    /*public void Remove(int v)
    {
        var reader = GetBitWriter();
        var remaining = Size - (int)reader.BaseStream.Position - v;
        var add = Data.Skip((int)reader.BaseStream.Position + 4).Take(remaining).Reverse().ToArray();
        reader.WriteBytes(add);
        WriteSize((int)reader.BaseStream.Position);
        GetBitWriter().BaseStream.Position = Size - remaining;
    }*/

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