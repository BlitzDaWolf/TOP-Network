using System;
using System.Buffers.Binary;
using TOP_Network.Extention;

namespace TOP_Network.Packets.Streams;

public class PacketReader
{
    private readonly PacketStream Stream;
    public PacketStream BaseStream => Stream;

    public PacketReader(PacketStream stream)
    {
        Stream = stream;
    }

    public byte ReadByte()
    {
        return Stream.data[Stream.Position++];
    }
    public sbyte ReadSByte()
    {
        return (sbyte)Stream.data[Stream.Position++];
    }


    public byte[] ReadBytes(int size)
    {
        var a = Stream.data.Skip(Stream.Position).Take(size).ToArray();
        Stream.Position += size;
        return a;
    }
    public byte[] ReadBytes(int size, bool small = false)
    {
        var a = Stream.data.Skip(Stream.Position).Take(size).ToArray();
        Stream.Position += size;
        return small ? a.Reverse().ToArray() : a;
    }



    public T? ReadType<T>() => (T?)this.ReadType(typeof(T));
/*    public byte[] ReadBytes(this PacketReader reader, int size, bool reverse = false)
    {
        return reverse ? this.ReadBytes(size).Reverse().ToArray() : this.ReadBytes(size);
    }*/

    public object? ReadType(Type type, bool small = false)
    {
        if (type == typeof(string)) return string.Join("", this.ReadBytes((short)this.ReadType(typeof(short))!).Select(x => (char)x));
        if (type == typeof(byte[])) return this.ReadBytes((short)this.ReadType(typeof(short))!);
        if (type == typeof(DateTime)) return new DateTime(this.ReadType<long>());
        if (type == typeof(bool)) return this.ReadByte() == 1;

        // byte
        if (type == typeof(byte)) return this.ReadByte();
        if (type == typeof(sbyte)) return this.ReadSByte();

        var byteLength = type.SizeOf();

        // short
        if (type == typeof(short)) return BinaryPrimitives.ReadInt16BigEndian(this.ReadBytes(byteLength, small));
        if (type == typeof(ushort)) return BinaryPrimitives.ReadUInt16BigEndian(this.ReadBytes(byteLength, small));

        // Int
        if (type == typeof(int)) return BinaryPrimitives.ReadInt32BigEndian(this.ReadBytes(byteLength, small));
        if (type == typeof(uint)) return BinaryPrimitives.ReadUInt32BigEndian(this.ReadBytes(byteLength, small));


        if (type == typeof(float)) return BinaryPrimitives.ReadSingleBigEndian(this.ReadBytes(byteLength, small)); // float
        if (type == typeof(double)) return BinaryPrimitives.ReadDoubleBigEndian(this.ReadBytes(byteLength, small)); // double

        // Long
        if (type == typeof(long)) return BinaryPrimitives.ReadInt64BigEndian(this.ReadBytes(byteLength, small));
        if (type == typeof(ulong)) return BinaryPrimitives.ReadUInt64BigEndian(this.ReadBytes(byteLength, small));

        return null;
        // throw new BinaryException(type);
    }
}
