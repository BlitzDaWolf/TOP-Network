using System.Buffers.Binary;
using System.Reflection.PortableExecutable;
using TOP_Records;

namespace TOP_Network.Extention
{
    public static class BinaryExtention
    {
        public static object ReadType(this BinaryReader reader, Type type)
        {
            if (type == typeof(string))return reader.ReadString((short)reader.ReadType(typeof(short)));
            if(type == typeof(byte[])) return reader.ReadBytes((short)reader.ReadType(typeof(short)));

            // byte
            if (type == typeof(byte)) return (reader.ReadByte());
            if (type == typeof(sbyte)) return (reader.ReadSByte());

            // short
            if (type == typeof(short)) return BinaryPrimitives.ReadInt16BigEndian(reader.ReadBytes(2));
            if (type == typeof(ushort)) return BinaryPrimitives.ReadUInt16BigEndian(reader.ReadBytes(2));

            // Int
            if (type == typeof(int)) return BinaryPrimitives.ReadInt32BigEndian(reader.ReadBytes(4));
            if (type == typeof(uint)) return BinaryPrimitives.ReadUInt32BigEndian(reader.ReadBytes(4));

            
            if (type == typeof(float)) return BinaryPrimitives.ReadSingleBigEndian(reader.ReadBytes(4)); // float
            if (type == typeof(double)) return BinaryPrimitives.ReadDoubleBigEndian(reader.ReadBytes(4)); // double

            // Long
            if (type == typeof(long)) return BinaryPrimitives.ReadInt64BigEndian(reader.ReadBytes(8));
            if (type == typeof(ulong)) return BinaryPrimitives.ReadUInt64BigEndian(reader.ReadBytes(8));
            if (type == typeof(bool)) return reader.ReadBoolean();

            throw new Exception($"The type ({type}) has not been implemented");
        }


        public static void WriteType(this BinaryWriter writer, object value)
        {
            var start = writer.BaseStream.Position;

            var type = value.GetType();
            if (type == typeof(string))
            {
                var str = (string)value;
                writer.WriteType(str.Select(x => (byte)x).ToArray());
            }
            if(type == typeof(byte[]))
            {
                var str = (byte[])value;
                writer.WriteType((short)str.Length);
                writer.Write(str);
            }

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

            if (writer.BaseStream.Position == start) throw new Exception($"The type ({type}) has not been implemented");
        }

        public static void WriteBytes(this BinaryWriter writer, byte[] data)
        {
            writer.Write(data.Reverse().ToArray());
        }
    }
}
