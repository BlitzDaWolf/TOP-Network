using System.Reflection;
using TOP_Network.Enum;
using TOP_Network.Extention;
using TOP_Network.Packets;
using TOP_Records.Tables;
using static System.Net.Mime.MediaTypeNames;

namespace TOP_Network.Converter
{
    public static class PacketToClass
    {
        public static T Convert<T>(this Packet packet)
        {
            Dictionary<PropertyInfo, object> values = new Dictionary<PropertyInfo, object>();

            using var reader = packet.GetBitReader();
            var command = (Commands)(short)reader.ReadType(typeof(short));

            return (T)reader.Read(typeof(T), values);
        }

        private static object Read(this BinaryReader reader, Type type, Dictionary<PropertyInfo, object> values)
        {
            Dictionary<PropertyInfo, object> test = new Dictionary<PropertyInfo, object>(values);
            var entity = Activator.CreateInstance(type)!;
            var properties = type.GetProperties();

            foreach (var item in properties)
            {
                if (item.PropertyType.IsArray)
                {
                    if (test.ContainsKey(item)) continue;
                    test.Add(item, reader.ReadArry(item, test));
                    item.SetValue(entity, test[item]);
                }
                else
                {
                    if (test.ContainsKey(item)) continue;
                    test.Add(item, reader.ReadSingle(item, test));
                    item.SetValue(entity, test[item]);
                }
            }

            return entity;
        }

        public static object ReadSingle(this BinaryReader reader, PropertyInfo info, Dictionary<PropertyInfo, object> values)
        {
            try
            {
                return reader.ReadType(info.PropertyType);
            }
            catch
            {
                return reader.Read(info.PropertyType, values);
            }
        }

        public static object ReadArry(this BinaryReader reader, PropertyInfo info, Dictionary<PropertyInfo, object> values)
        {
            try
            {
                return reader.ReadType(info.PropertyType);
            }
            catch
            {
                var size = (byte)reader.ReadType(typeof(byte));
                var type = info.PropertyType.GetElementType()!;
                var value = Array.CreateInstance(type, size);

                for (short i = 0; i < size; i++)
                {
                    value.SetValue(reader.Read(type, values), i);
                }

                return value;
            }
        }
    }
}
