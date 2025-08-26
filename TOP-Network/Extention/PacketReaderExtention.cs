using System;
using System.Buffers.Binary;
using TOP_Network.Packets;
using TOP_Network.Packets.Streams;

namespace TOP_Network.Extention;

public static class BinaryExtention
{

    public static bool WriteType(this PacketWriter writer, object value, bool small = false)
    {
        return writer.WriteType(value, value.GetType());
    }

    public static bool WriteType(this PacketWriter writer, object value, Type type)
    {
        var start = writer.BaseStream.Position;

        if (type == typeof(string))
        {
            var str = (string)value;
            writer.WriteType(str.Select(x => (byte)x).ToArray());
        }
        if (type == typeof(byte[]))
        {
            var str = (byte[])value;
            writer.WriteType((short)str.Length);
            writer.WriteBytes(str);
        }

        if (type == typeof(DateTime)) WriteType(writer, ((DateTime)value).Ticks);

        // byte
        if (type == typeof(byte)) writer.Write((byte)value);
        // if (type == typeof(sbyte)) writer.Write((sbyte)value);

        // short
        if (type == typeof(short)) writer.WriteBytes(BitConverter.GetBytes((short)value).Reverse().ToArray());
        if (type == typeof(ushort)) writer.WriteBytes(BitConverter.GetBytes((ushort)value).Reverse().ToArray());

        // Int
        if (type == typeof(int)) writer.WriteBytes(BitConverter.GetBytes((int)value).Reverse().ToArray());
        if (type == typeof(uint)) writer.WriteBytes(BitConverter.GetBytes((uint)value).Reverse().ToArray());


        if (type == typeof(float)) writer.WriteBytes(BitConverter.GetBytes((float)value).Reverse().ToArray()); // float
        if (type == typeof(double)) writer.WriteBytes(BitConverter.GetBytes((double)value).Reverse().ToArray()); // double

        // Long
        if (type == typeof(long)) writer.WriteBytes(BitConverter.GetBytes((long)value).Reverse().ToArray());
        if (type == typeof(ulong)) writer.WriteBytes(BitConverter.GetBytes((ulong)value).Reverse().ToArray());

        if (writer.BaseStream.Position == start) return false;
        return true;
    }

    public static void WriteBytes(this BinaryWriter writer, byte[] data, bool small = false)
    {
        if (small)
            writer.Write(data.Reverse().ToArray());
        else
            writer.Write(data.Reverse().ToArray());
    }
}
