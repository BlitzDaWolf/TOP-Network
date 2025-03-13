using System.Buffers.Binary;
using System.Reflection.PortableExecutable;
using TOP_Network.Exceptions;
using TOP_Records;

namespace TOP_Network.Extention
{
    public static class BinaryExtention
    {
        public static T ReadType<T>(this BinaryReader reader) => (T)reader.ReadType(typeof(T));

        public static object ReadType(this BinaryReader reader, Type type)
        {
            if (type == typeof(string))return reader.ReadString((short)reader.ReadType(typeof(short)));
            if(type == typeof(byte[])) return reader.ReadBytes((short)reader.ReadType(typeof(short)));
            if (type == typeof(DateTime)) return new DateTime(reader.ReadType<long>());
            if (type == typeof(bool)) return reader.ReadBoolean();

            // byte
            if (type == typeof(byte)) return (reader.ReadByte());
            if (type == typeof(sbyte)) return (reader.ReadSByte());

            var byteLength = type.SizeOf();

            // short
            if (type == typeof(short)) return BinaryPrimitives.ReadInt16BigEndian(reader.ReadBytes(byteLength));
            if (type == typeof(ushort)) return BinaryPrimitives.ReadUInt16BigEndian(reader.ReadBytes(byteLength));

            // Int
            if (type == typeof(int)) return reader.ReadInt();
            if (type == typeof(uint)) return reader.ReadUInt();

            
            if (type == typeof(float)) return BinaryPrimitives.ReadSingleBigEndian(reader.ReadBytes(byteLength)); // float
            if (type == typeof(double)) return BinaryPrimitives.ReadDoubleBigEndian(reader.ReadBytes(byteLength)); // double

            // Long
            if (type == typeof(long)) return reader.ReadLong();
            if (type == typeof(ulong)) return reader.ReadULong();

            throw new BinaryException(type);
        }

        #region
        private static int ReadInt(this BinaryReader reader) => BinaryPrimitives.ReadInt32BigEndian(reader.ReadBytes(sizeof(int)));
        private static uint ReadUInt(this BinaryReader reader) => BinaryPrimitives.ReadUInt32BigEndian(reader.ReadBytes(sizeof(uint)));
        private static long ReadLong(this BinaryReader reader) => BinaryPrimitives.ReadInt64BigEndian(reader.ReadBytes(sizeof(long)));
        private static ulong ReadULong(this BinaryReader reader) => BinaryPrimitives.ReadUInt64BigEndian(reader.ReadBytes(sizeof(ulong)));
        #endregion


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

            if(type == typeof(DateTime)) WriteType(writer, ((DateTime)value).Ticks);

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

            if (writer.BaseStream.Position == start) throw new BinaryException(type);
        }

        public static void WriteBytes(this BinaryWriter writer, byte[] data)
        {
            writer.Write(data.Reverse().ToArray());
        }
    }
}
