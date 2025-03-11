using System.Reflection;
using TOP_Network.Extention;
using TOP_Network.Packets;

namespace TOP_Network.Converter
{
    public static class PacketToClass
    {
        public static T Convert<T>(this Packet packet)
        {
            Dictionary<PropertyInfo, object> values = new Dictionary<PropertyInfo, object>();

            using var reader = packet.GetBitReader();

            return (T)reader.Read(typeof(T), values);
        }

        private static object Read(this BinaryReader reader, Type type, Dictionary<PropertyInfo, object> values)
        {
            var entity = Activator.CreateInstance(type)!;
            var properties = type.GetProperties();

            foreach (var item in properties)
            {
                if (values.ContainsKey(item)) continue;
                values.Add(item, reader.ReadType(item.PropertyType));
                item.SetValue(entity, values[item]);
            }

            return entity;
        }
    }
}
