using System;
using TOP_Network.Extention;
using TOP_Network.Interfaces.Packets;

namespace TOP_Network.Packets;

public class RPacket : Packet, IRPacket
{
    public int ReversePoint { get; private set; } = 0;

    public RPacket() : base() { }

    public override void Init(byte[] data)
    {
        base.Init(data);
        GetStream().Position = StartSize + 6;
    }

    public void UnRead(int v)
    {
        GetReader().BaseStream.Position = GetReader().BaseStream.Position - v;
    }

    public override void RemoveLast(int amount)
    {
        base.RemoveLast(amount);
        ReversePoint -= amount;
        ReversePoint = Math.Max(ReversePoint, 0);
    }

    public byte ReadChar() => GetReader().ReadByte();
    public int ReadLong() => GetReader().ReadType<int>();
    public uint ReadULong() => GetReader().ReadType<uint>();
    public short ReadShort() => GetReader().ReadType<short>();
    public ushort ReadUShort() => GetReader().ReadType<ushort>();
    public long ReadLongLong() => GetReader().ReadType<long>();
    public ulong ReadULongLong() => GetReader().ReadType<ulong>();
    public string ReadString() => GetReader().ReadType<string>()!.Replace("\0", "");
    public byte[] ReadSeq() => GetReader().ReadType<byte[]>()!;

    public byte ReverseReadChar()
    {
        var reader = GetReader();
        var currentPos = reader.BaseStream.Position;
        reader.BaseStream.Position = Size - ReversePoint - 1;

        var val = ReadChar();
        ReversePoint++;
        reader.BaseStream.Position = currentPos;

        return val;
    }

    public int ReverseReadLong()
    {
        var reader = GetReader();
        var currentPos = reader.BaseStream.Position;
        reader.BaseStream.Position = Size - ReversePoint - 4;

        var val = ReadLong();
        ReversePoint+=4;
        reader.BaseStream.Position = currentPos;

        return val;
    }

    public uint ReverseReadULong()
    {
        var reader = GetReader();
        var currentPos = reader.BaseStream.Position;
        reader.BaseStream.Position = Size - ReversePoint - 4;

        var val = ReadULong();
        ReversePoint+=4;
        reader.BaseStream.Position = currentPos;

        return val;
    }

    public short ReverseReadShort()
    {
        var reader = GetReader();
        var currentPos = reader.BaseStream.Position;
        reader.BaseStream.Position = Size - ReversePoint - 2;

        var val = ReadShort();
        ReversePoint+=2;
        reader.BaseStream.Position = currentPos;

        return val;
    }

    public ushort ReverseReadUShort()
    {
        var reader = GetReader();
        var currentPos = reader.BaseStream.Position;
        reader.BaseStream.Position = Size - ReversePoint - 2;

        var val = ReadUShort();
        ReversePoint+=2;
        reader.BaseStream.Position = currentPos;

        return val;
    }

    public long ReverseReadLongLong()
    {
        var reader = GetReader();
        var currentPos = reader.BaseStream.Position;
        reader.BaseStream.Position = Size - ReversePoint - 8;

        var val = ReadLongLong();
        ReversePoint+=8;
        reader.BaseStream.Position = currentPos;

        return val;
    }

    public ulong ReverseReadULongLong()
    {
        var reader = GetReader();
        var currentPos = reader.BaseStream.Position;
        reader.BaseStream.Position = Size - ReversePoint - 8;

        var val = ReadULongLong();
        ReversePoint+=8;
        reader.BaseStream.Position = currentPos;

        return val;
    }

    public string ReverseReadString()
    {
        var reader = GetReader();
        var currentPos = reader.BaseStream.Position;
        var size = ReverseReadShort();
        reader.BaseStream.Position = Size - ReversePoint - size - 2;

        var val = ReadString();
        ReversePoint+=size+2;
        reader.BaseStream.Position = currentPos;

        return val;
    }

    public byte[] ReverseReadSeq()
    {
        var reader = GetReader();
        var currentPos = reader.BaseStream.Position;
        var size = ReverseReadShort();
        reader.BaseStream.Position = Size - ReversePoint - size - 2;

        var val = ReadSeq();
        ReversePoint+=size+2;
        reader.BaseStream.Position = currentPos;

        return val;
    }
}