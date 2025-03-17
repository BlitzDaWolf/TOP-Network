using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using TOP_Network.Attributes;
using TOP_Network.Enum;
using TOP_Network.Exceptions;
using TOP_Network.Extention;
using TOP_Network.Packets;
using TOP_Records;
using static System.Net.Mime.MediaTypeNames;
using TOP_Records.Tables;
using System.Xml;

namespace TOP_Network.Converter
{
    class PacketReadS
    {
        public BinaryReader Reader { get; set; }
        public Dictionary<PropertyInfo, object> Values { get; set; }
    }

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

            try
            {
                var command = (Commands)(short)reader.ReadType(typeof(short));
                if (!keyValuePairs.ContainsKey(command))
                {
                    throw new Exception($"Command [{command}] was not found");
                }
                var result = reader.Read(keyValuePairs[command], values);
                if (reader.BaseStream.Position != packet.Size)
                {
                    var h = packet.DisplayHex();
                    var missed = packet.Size - reader.BaseStream.Position;

                    reader.ReadBytes((int)missed);

                    throw new NotFullyReadException(result);
                }

                return result;
            }
            catch (NotFullyReadException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                if (reader.BaseStream.Position != packet.Size)
                {
                    var missed = packet.Size - reader.BaseStream.Position;
                    reader.ReadBytes((int)missed);
                }
                throw e;
            }
        }

        public static object Read(this BinaryReader reader, Type type, Dictionary<PropertyInfo, object> values)
        {
            Dictionary<PropertyInfo, object> test = new Dictionary<PropertyInfo, object>(values);
            var entity = Activator.CreateInstance(type)!;
            var properties = type.GetProperties();

            foreach (var item in properties)
            {
                if (!reader.ReadValid(item, entity))
                {
                    continue;
                }
                if(reader.ReadChooise(item, test, entity))
                {
                    continue;
                }
                var i = reader.ReadIf(item, test);
                if (i != 0)
                {
                    if(i == 1) continue;
                    if (item.GetCustomAttribute<EndIfAttribute>() != null)
                    {

                        if (reader._Read(item, test, entity)) { }
                        return entity;
                    }
                }
                else
                {
                }

                if(reader._Read(item, test, entity))
                {

                }
            }

            return entity;
        }

        private static int ReadIf(this BinaryReader reader, PropertyInfo info, Dictionary<PropertyInfo, object> values)
        {
            List<IfAttribute?> ifs = info.GetCustomAttributes(typeof(IfAttribute)).Select(x => x as IfAttribute).ToList();
            if (ifs.Count == 0) return 0;

            var vals = ifs.FirstOrDefault(x => x.A(values.FirstOrDefault(y => y.Key.Name == x.v1).Value));

            return vals != null ? 2 : 1;
        }

        private static bool ReadChooise(this BinaryReader reader, PropertyInfo info, Dictionary<PropertyInfo, object> values, object entity)
        {

            List<ChooseAttribute?> choises = info.GetCustomAttributes(typeof(ChooseAttribute)).Select(x => x as ChooseAttribute).ToList();
            if (choises.Count > 0)
            {
                var choiseSelect = (byte)reader.ReadType(typeof(byte));
                var choise = choises.FirstOrDefault(x => x.Value == choiseSelect);
                if (choise != null)
                {
                    var r = reader.Read(choise.DataType, values);
                    info.SetValue(entity, r);
                }
                return true;
            }
            return false;
        }

        private static bool ReadValid(this BinaryReader reader, PropertyInfo info, object entity)
        {
            // Get validRecordAttribute
            // Check if there is an attribute
            var valids = info.GetCustomAttributes(typeof(ValidRecordAttribute)).FirstOrDefault() as ValidRecordAttribute;
            if (valids == null) return true;

            // Get Record ID
            if (info.PropertyType != typeof(int) && info.PropertyType != typeof(short)) throw new WrongTypeExcetion($"Invalid type `{info.PropertyType}`");
            int id;
            if (info.PropertyType == typeof(int))
            {
                id = reader.ReadType<int>();
                info.SetValue(entity, id);
            }
            else
            {
                id = reader.ReadType<short>();
                info.SetValue(entity, (short)id);
            }

            // Check if record exsist
            // Return `True` if exist otherwise return `False`
            return RecorReaders.GetRecord(valids.RecoredTable, id) == null;
        }

        private static bool _Read(this BinaryReader reader, PropertyInfo info, Dictionary<PropertyInfo, object> values, object entity)
        {
            if (info.PropertyType.IsArray)
            {
                if (values.ContainsKey(info)) return false;
                values.Add(info, reader.ReadArry(info, values));
                info.SetValue(entity, values[info]);
            }
            else
            {
                if (values.ContainsKey(info)) return false;
                values.Add(info, reader.ReadSingle(info, values));
                info.SetValue(entity, values[info]);
            }
            return true;
        }

        #region test
        private static int convertToInt(object source)
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

        private static object ReadArry(this BinaryReader reader, PropertyInfo info, Dictionary<PropertyInfo, object> values)
        {
            var res = reader.ReadType(info.PropertyType);
            if (res == null)
            {
                int size = 0;
                ArraySizeAttribute? t = info.GetCustomAttribute<ArraySizeAttribute>() as ArraySizeAttribute;
                ArrayLengthAttribute? al = info.GetCustomAttribute<ArrayLengthAttribute>() as ArrayLengthAttribute;
                if (t != null)
                {
                    size = reader.ReadType(t.ReadType).GetHashCode();
                }
                else if (al != null)
                {
                    size = al.Length;
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

        private static object ReadSingle(this BinaryReader reader, PropertyInfo info, Dictionary<PropertyInfo, object> values)
        {
            var res = reader.ReadType(info.PropertyType, info.GetCustomAttributes(typeof(SmallEndeanAttribute)).FirstOrDefault() != null);
            if (res == null)
            {
                res = reader.Read(info.PropertyType, values);
            }
            return res;
        }

        public static bool HasCommand(Commands command)
        {
            return keyValuePairs.ContainsKey(command);
        }
        #endregion

    }
}
