using System;
using System.Buffers.Binary;
using TOP_Network.Packets;
using TOP_Network.Packets.Streams;

namespace TOP_Network.Extention;

public static class BinaryExtention
{

    public static bool WriteType(this PacketWriter writer, object value, bool small = false)
    {
        var start = writer.BaseStream.Position;

        var type = value.GetType();
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
        if (type == typeof(sbyte)) writer.Write((sbyte)value);

        // short
        if (type == typeof(short)) writer.WriteBytes(BitConverter.GetBytes((short)value));
        if (type == typeof(ushort)) writer.WriteBytes(BitConverter.GetBytes((ushort)value));

        // Int
        if (type == typeof(int)) writer.WriteBytes(BitConverter.GetBytes((int)value));
        if (type == typeof(uint)) writer.WriteBytes(BitConverter.GetBytes((uint)value));


        if (type == typeof(float)) writer.WriteBytes(BitConverter.GetBytes((float)value)); // float
        if (type == typeof(double)) writer.WriteBytes(BitConverter.GetBytes((double)value)); // double

        // Long
        if (type == typeof(long)) writer.WriteBytes(BitConverter.GetBytes((long)value));
        if (type == typeof(ulong)) writer.WriteBytes(BitConverter.GetBytes((ulong)value));
        if (type == typeof(bool)) writer.Write((bool)value);

        if (writer.BaseStream.Position == start) return false;
        return true;
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
        if (type == typeof(sbyte)) writer.Write((sbyte)value);

        // short
        if (type == typeof(short)) writer.WriteBytes(BitConverter.GetBytes((short)value));
        if (type == typeof(ushort)) writer.WriteBytes(BitConverter.GetBytes((ushort)value));

        // Int
        if (type == typeof(int)) writer.WriteBytes(BitConverter.GetBytes((int)value));
        if (type == typeof(uint)) writer.WriteBytes(BitConverter.GetBytes((uint)value));


        if (type == typeof(float)) writer.WriteBytes(BitConverter.GetBytes((float)value)); // float
        if (type == typeof(double)) writer.WriteBytes(BitConverter.GetBytes((double)value)); // double

        // Long
        if (type == typeof(long)) writer.WriteBytes(BitConverter.GetBytes((long)value));
        if (type == typeof(ulong)) writer.WriteBytes(BitConverter.GetBytes((ulong)value));
        if (type == typeof(bool)) writer.Write((bool)value);

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
