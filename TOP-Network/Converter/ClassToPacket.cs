using System.Reflection;
using System.Reflection.PortableExecutable;
using TOP_Network.Attributes;
using TOP_Network.Enum;
using TOP_Network.Extention;
using TOP_Network.Packets;
using TOP_Records;
using TOP_Records.Tables;
using static System.Net.Mime.MediaTypeNames;

namespace TOP_Network.Converter
{
    public static class ClassToPacket
    {
        public static Packet Convert(this object entity, Commands command)
        {
            Dictionary<PropertyInfo, object> values = [];

            Packet packet = new();
            packet.Init(new byte[4096]);
            using BinaryWriter writer = packet.GetBitWriter();
            writer.BaseStream.Position = 0;

            writer.WriteType(50);
            writer.WriteType(-2147483648);
            writer.WriteType((short)command);

            writer.WriteData(entity, values);

            var size = (int)packet.GetStream().Position;
            packet.GetStream().Position = 0;
            writer.WriteType(size);

            return packet;
        }

        private static void WriteData(this BinaryWriter writer, object entity, Dictionary<PropertyInfo, object> values)
        {
            Dictionary<PropertyInfo, object> test = new(values);
            var properties = entity.GetType().GetProperties();

            foreach (var item in properties)
            {
                if (item.GetCustomAttribute<ValidRecordAttribute>() is ValidRecordAttribute valid)
                {
                    if (item.PropertyType != typeof(int)) throw new Exception($"Invalid type `{item.PropertyType}`");

                    var id = (int)item.GetValue(entity)!;
                    if (RecorReaders.GetRecord(valid.RecoredTable, id) == null)
                    {
                        return;
                    }
                    continue;
                }

                if (test.ContainsKey(item)) continue;
                test.Add(item, item.GetValue(entity)!);
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
