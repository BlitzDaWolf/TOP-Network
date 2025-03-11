using System.Reflection;
using TOP_Network.Extention;
using TOP_Network.Packets;
using TOP_Records.Tables;
using static System.Net.Mime.MediaTypeNames;

namespace TOP_Network.Converter
{
    public static class ClassToPacket
    {
        public static Packet Convert(this object entity)
        {
            Dictionary<PropertyInfo, object> values = new Dictionary<PropertyInfo, object>();

            Packet packet = new Packet();
            packet.Init(new byte[4096]);
            using BinaryWriter writer = packet.GetBitWriter();
            writer.BaseStream.Position = 0;

            writer.WriteType(50);
            writer.WriteType(-2147483648);
            writer.WriteType((short)0);

            writer.WriteData(entity, values);

            var size = (int)packet.GetStream().Position;
            packet.GetStream().Position = 0;
            writer.WriteType(size);

            return packet;
        }

        private static void WriteData(this BinaryWriter writer, object type, Dictionary<PropertyInfo, object> values)
        {
            Dictionary<PropertyInfo, object> test = new Dictionary<PropertyInfo, object>(values);
            var properties = type.GetType().GetProperties();

            foreach (var item in properties)
            {
                if (test.ContainsKey(item)) continue;
                test.Add(item, item.GetValue(type)!);
                if (item.PropertyType.IsArray)
                {
                    writer.WriteArry(item, test);
                }
                else
                {
                    writer.WriteSingle(item, test);
                }
            }
        }

        private static void WriteSingle(this BinaryWriter writer, PropertyInfo info, Dictionary<PropertyInfo, object> values)
        {
            try
            {
                writer.WriteType(values[info]);
            }
            catch
            {
                writer.WriteData(values[info], values);
            }
        }

        public static void WriteArry(this BinaryWriter writer, PropertyInfo info, Dictionary<PropertyInfo, object> values)
        {
            try
            {
                writer.WriteType(values[info]);
            }
            catch
            {
                var arr = (Array)values[info];
                writer.WriteType((byte)arr.Length);
                for (int i = 0; i < arr.Length; i++)
                {
                    writer.WriteData(arr.GetValue(i)!, values);
                }
            }
        }
    }
}
