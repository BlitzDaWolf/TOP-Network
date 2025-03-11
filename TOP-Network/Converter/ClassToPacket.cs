using System.Reflection;
using TOP_Network.Extention;
using TOP_Network.Packets;

namespace TOP_Network.Converter
{
    public static class ClassToPacket
    {
        public static Packet Convert(this object entity)
        {
            Dictionary<PropertyInfo, object> values = new Dictionary<PropertyInfo, object>();

            Packet packet = new Packet();
            packet.Init(new byte[2048]);
            using BinaryWriter writer = packet.GetBitWriter();
            writer.BaseStream.Position = 0;

            writer.WriteType(50);
            writer.WriteType(-2147483648);
            writer.WriteType((short)0);

            writer.ReadData(entity, values);

            var size = (int)packet.GetStream().Position;
            packet.GetStream().Position = 0;
            writer.WriteType(size);

            return packet;
        }

        private static void ReadData(this BinaryWriter writer, object type, Dictionary<PropertyInfo, object> values)
        {
            var properties = type.GetType().GetProperties();

            foreach (var item in properties)
            {
                if (values.ContainsKey(item)) continue;
                values.Add(item, item.GetValue(type)!);

                writer.WriteType(values[item]);
            }
        }
    }
}
