using System.Reflection;
using TOP_Network.Attributes;
using TOP_Network.Enum;
using TOP_Network.Exceptions;
using TOP_Network.Extention;
using TOP_Network.Packets;
using TOP_Records;

namespace TOP_Network.Converter
{
    public static class PacketToClass
    {
        public static T Convert<T>(this Packet packet)
        {
            Dictionary<PropertyInfo, object> values = new Dictionary<PropertyInfo, object>();

            using var reader = packet.GetBitReader();
            var command = (Commands)(short)reader.ReadType(typeof(short));

            T result = (T)reader.Read(typeof(T), values);

            if(packet.GetStream().Position != packet.Size)
            {
                // throw new NotFullyReadException(packet);
            }

            return result;
        }

        private static object Read(this BinaryReader reader, Type type, Dictionary<PropertyInfo, object> values)
        {
            Dictionary<PropertyInfo, object> test = new Dictionary<PropertyInfo, object>(values);
            var entity = Activator.CreateInstance(type)!;
            var properties = type.GetProperties();

            foreach (var item in properties)
            {
                ValidRecordAttribute? valid = item.GetCustomAttributes(typeof(ValidRecordAttribute)).FirstOrDefault() as ValidRecordAttribute;
                if (valid != null)
                {
                    if (item.PropertyType != typeof(int) && item.PropertyType != typeof(short)) throw new WrongTypeExcetion($"Invalid type `{item.PropertyType}`");

                    int id = 0;
                    if (item.PropertyType == typeof(int))
                        id = (int)reader.ReadType(typeof(int));
                    else
                        id = (short)reader.ReadType(typeof(short));

                    item.SetValue(entity, id);
                    if(RecorReaders.GetRecord(valid.RecoredTable, id) == null)
                    {
                        return null;
                    }
                    continue;
                }
                List<ChooseAttribute?> choises = item.GetCustomAttributes(typeof(ChooseAttribute)).Select(x => x as ChooseAttribute).ToList();
                if (choises.Count > 0)
                {
                    var choiseSelect = (byte)reader.ReadType(typeof(byte));
                    var choise = choises.FirstOrDefault(x => x.Value == choiseSelect);
                    if(choise != null)
                    {
                        var r = reader.Read(choise.DataType, values);
                        item.SetValue(entity, r);
                    }
                    continue;
                }

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
                Array value = Array.CreateInstance(type, size);

                for (short i = 0; i < size; i++)
                {
                    var r = reader.Read(type, values);
                    value.SetValue(r, i);
                }

                return value;
            }
        }
    }
}
