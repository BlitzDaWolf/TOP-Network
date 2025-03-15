using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
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
        private static Dictionary<Commands, Type> keyValuePairs = new Dictionary<Commands, Type>();

        public static void AddType<T>(Commands command)
        {
            AddType(typeof(T), command);
        }
        public static void AddType(Type type, Commands command)
        {
            keyValuePairs.Add(command, type);
        }

        public static T Convert<T>(this Packet packet)
        {
            Dictionary<PropertyInfo, object> values = new Dictionary<PropertyInfo, object>();

            using var reader = packet.GetBitReader();
            reader.ReadType(typeof(int));
            reader.ReadType(typeof(int));

            var command = (Commands)(short)reader.ReadType(typeof(short));
            T result;
            try
            {
                result = (T)reader.Read(typeof(T), values);
                if (packet.GetStream().Position != packet.Size)
                {
                    var h = packet.DisplayHex();
                    var missed = packet.Size - packet.GetStream().Position;
                    throw new NotFullyReadException(result);
                }

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(packet.DisplayHex());
                throw e;
            }
        }

        public static object? Convert(this Packet packet)
        {
            Dictionary<PropertyInfo, object> values = new Dictionary<PropertyInfo, object>();

            using var reader = packet.GetBitReader();
            reader.ReadType(typeof(int));
            reader.ReadType(typeof(int));

            var command = (Commands)(short)reader.ReadType(typeof(short));

            try
            {
                if (!keyValuePairs.ContainsKey(command))
                {
                    throw new Exception($"Command [{command}] was not found");
                }
                var result = reader.Read(keyValuePairs[command], values);
                if (reader.BaseStream.Position != packet.Size)
                {
                    var h = packet.DisplayHex();
                    var missed = packet.Size - reader.BaseStream.Position;
                    throw new NotFullyReadException(result);
                }

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
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
                List<IfAttribute?> ifs = item.GetCustomAttributes(typeof(IfAttribute)).Select(x => x as IfAttribute).ToList();
                if(ifs.Count > 0)
                {
                    var vals = ifs.FirstOrDefault(x => x.A(test.FirstOrDefault(y => y.Key.Name == x.v1).Value));
                    if(vals != null)
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
                        if (item.GetCustomAttribute<EndIfAttribute>() != null)
                            return entity;
                    }
                    else
                    {
                        continue;
                    }
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
            var res = reader.ReadType(info.PropertyType, info.GetCustomAttributes(typeof(SmallEndeanAttribute)).FirstOrDefault() != null);
            if (res == null)
            {
                res = reader.Read(info.PropertyType, values);
            }
            return res;
        }

        public static int convertToInt(object source)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using var ms = new MemoryStream();
            bf.Serialize(ms, source);
            var test = ms.ToArray();
            byte[] target = new byte[4];
            for (int i = 0; i < Math.Min(target.Length, test.Length); i++)
            {
                target[i] = test[i];
            }
            return BitConverter.ToInt32(target);
        }

        public static object ReadArry(this BinaryReader reader, PropertyInfo info, Dictionary<PropertyInfo, object> values)
        {
                var res = reader.ReadType(info.PropertyType);
                if (res == null)
                {
                    int size = 0;
                    ArraySizeAttribute? t = info.GetCustomAttribute<ArraySizeAttribute>() as ArraySizeAttribute;
                    if (t != null)
                    {
                        size = reader.ReadType(t.ReadType).GetHashCode();
                    }
                    else
                    {
                        size = (byte)reader.ReadType(typeof(byte));
                    }
                    var type = info.PropertyType.GetElementType()!;
                    Array value = Array.CreateInstance(type, size);

                    for (short i = 0; i < size; i++)
                    {
                        var r = reader.Read(type, values);
                        value.SetValue(r, i);
                    }

                    return value;
                }
                return res;
        }
    }
}
