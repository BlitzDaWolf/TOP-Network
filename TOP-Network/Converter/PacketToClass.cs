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
    class PacketReadS
    {
        public BinaryReader Reader { get; set; }
        public Dictionary<PropertyInfo, object> Values { get; set; }
    }

    public static class PacketToClass
    {
        private static Dictionary<Commands, Type> keyValuePairs = new();

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
            Dictionary<PropertyInfo, object> values = new();

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
            catch (Exception)
            {
                Console.WriteLine(packet.DisplayHex());
                throw;
            }
        }

        public static object? Convert(this Packet packet)
        {
            Dictionary<PropertyInfo, object> values = [];

            using var reader = packet.GetBitReader();
            reader.ReadType(typeof(int));
            reader.ReadType(typeof(int));

            try
            {
                var command = (Commands)(short)reader.ReadType(typeof(short))!;
                if (!keyValuePairs.ContainsKey(command))
                {
                    throw new Exception($"Command [{command}] was not found");
                }
                var result = reader.Read(keyValuePairs[command], values);
                if (reader.BaseStream.Position != packet.Size)
                {
                    var h = packet.DisplayHex();
                    var missed = packet.Size - reader.BaseStream.Position;

                    var m = reader.ReadBytes((int)missed);

                    throw new NotFullyReadException(result);
                }

                return result;
            }
            catch (NotFullyReadException)
            {
                throw;
            }
            catch (Exception)
            {
                if (reader.BaseStream.Position != packet.Size)
                {
                    var missed = packet.Size - reader.BaseStream.Position;
                    reader.ReadBytes((int)missed);
                }
                throw;
            }
        }

        public static object Read(this BinaryReader reader, Type type, Dictionary<PropertyInfo, object> values)
        {
            Dictionary<PropertyInfo, object> test = new(values);
            var entity = Activator.CreateInstance(type)!;
            var properties = type.GetProperties();

            foreach (var item in properties)
            {
                var result = reader.ReadValid(item, entity);
                if (result == 2)
                {
                    return entity;
                }
                else if(result == 1)
                {
                    test.Add(item, item.GetValue(entity));
                    continue;
                }
                else
                {

                }
                var ff = reader.ReadBreak(item, test, entity);
                if (ff != null)
                {
                    if (ff == true)
                    {
                        break;
                    }
                    else
                    {
                        continue;
                    }
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
            List<IfAttribute> ifs = info.GetCustomAttributes<IfAttribute>().Where(x => x is not WhileAttribute).ToList();
            if (ifs.Count == 0) return 0;

            var tst = ifs.FirstOrDefault(x => x.v1.StartsWith("record."));
            if(tst != null)
            {
                var ty = values.Where(x => x.Key.GetCustomAttribute<ValidRecordAttribute>() != null).ToList();
                var recored = ty.Select(x => x.Key.GetCustomAttribute<ValidRecordAttribute>().GetRecord(x.Value.GetHashCode())).ToList();
                var valss = recored.Select(x => x.GetValue(tst.v1.Replace("record.", ""))).ToList();

                return valss.Where(x => tst.A(x)).Count() != 0 ? 2 : 1;
            }

            var value = ifs.Select(x => values.FirstOrDefault(y => y.Key.Name == x.v1)).ToList();
            var vals = ifs.FirstOrDefault(x => x.A(values.FirstOrDefault(y => y.Key.Name == x.v1).Value));

            return vals != null ? 2 : 1;
        }
        private static bool? ReadBreak(this BinaryReader reader, PropertyInfo info, Dictionary<PropertyInfo, object> values, object entity)
        {
            BreakIfAttribute? ifs = info.GetCustomAttribute<BreakIfAttribute>();
            if (ifs == null) return null;

            reader._Read(info, values, entity);
            var v = values[info];

            return ifs.Check(v);
        }

        private static bool ReadChooise(this BinaryReader reader, PropertyInfo info, Dictionary<PropertyInfo, object> values, object entity)
        {

            List<ChooseAttribute?> choises = info.GetCustomAttributes(typeof(ChooseAttribute)).Select(x => x as ChooseAttribute).ToList();
            if (choises.Count > 0)
            {
                var readType = choises[0]!.ReadType;
                var v = reader.ReadType(readType);
                var choiseSelect = v.GetHashCode();
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

        private static byte ReadValid(this BinaryReader reader, PropertyInfo info, object entity)
        {
            // Get validRecordAttribute
            // Check if there is an attribute
            var valids = info.GetCustomAttributes<ValidRecordAttribute>().FirstOrDefault();
            if (valids == null) return 0x00;

            // Get Record ID
            if (info.PropertyType != typeof(int) && info.PropertyType != typeof(short) && info.PropertyType != typeof(uint) && info.PropertyType != typeof(ushort)) throw new WrongTypeExcetion($"Invalid type `{info.PropertyType}`");
            int id;
            if (info.PropertyType == typeof(int))
            {
                id = (int)reader.ReadType(typeof(int), info.GetCustomAttributes(typeof(SmallEndeanAttribute)).FirstOrDefault() != null);
                info.SetValue(entity, id);
            }
            else if (info.PropertyType == typeof(short))
            {
                id = (short)reader.ReadType(typeof(short), info.GetCustomAttributes(typeof(SmallEndeanAttribute)).FirstOrDefault() != null);
                info.SetValue(entity, (short)id);
            }
            else if (info.PropertyType == typeof(ushort))
            {
                id = (ushort)reader.ReadType(typeof(ushort), info.GetCustomAttributes(typeof(SmallEndeanAttribute)).FirstOrDefault() != null);
                info.SetValue(entity, (ushort)id);
            }
            else
            {
                id = 0;
            }

            // Check if record exsist
            // Return `True` if exist otherwise return `False`
            return RecorReaders.GetRecord(valids.RecoredTable, id) == null ? (byte)2 : (byte)1;
        }

        private static bool _Read(this BinaryReader reader, PropertyInfo info, Dictionary<PropertyInfo, object> values, object entity)
        {
            if (values.ContainsKey(info)) return false;

            if (info.PropertyType.IsArray)
            {
                values.Add(info, reader.ReadArry(info, values));
                info.SetValue(entity, values[info]);
            }
            else
            {
                values.Add(info, reader.ReadSingle(info, values));
                info.SetValue(entity, values[info]);
            }
            return true;
        }

        #region test
        private static object? ReadArry(this BinaryReader reader, PropertyInfo info, Dictionary<PropertyInfo, object> values)
        {
            var res = reader.ReadType(info.PropertyType);
            if (res == null)
            {
                int size = 0;
                ArraySizeAttribute? t = info.GetCustomAttribute<ArraySizeAttribute>();
                ArrayLengthAttribute? al = info.GetCustomAttribute<ArrayLengthAttribute>();
                WhileAttribute? w = info.GetCustomAttribute<WhileAttribute>();
                if (t != null)
                {
                    size = reader.ReadType(t.ReadType, info.GetCustomAttributes(typeof(SmallEndeanAttribute)).FirstOrDefault() != null)!.GetHashCode();
                }
                else if(w != null)
                {
                    size = w.Max;
                }
                else if (al != null)
                {
                    size = al.Length;
                }
                else
                {
                    size = (byte)reader.ReadType(typeof(byte))!;
                }
                var type = info.PropertyType.GetElementType()!;
                Array value = Array.CreateInstance(type, size);

                if (w == null)
                {
                    for (short i = 0; i < size; i++)
                    {
                        var r = reader.Read(type, values);
                        value.SetValue(r, i);
                    }
                }
                else
                {
                    for (short i = 0; i < size; i++)
                    {
                        var v = reader.ReadType(w.ReadType);
                        if (!w.A(v)) break;
                        var r = reader.Read(type, values);
                        value.SetValue(r, v.GetHashCode());
                    }
                }

                return value;
            }
            return res;
        }

        private static object ReadSingle(this BinaryReader reader, PropertyInfo info, Dictionary<PropertyInfo, object> values)
        {

            if (info.GetCustomAttribute<ReadTypeAttribute>() != null)
            {
                var result = reader.ReadType(info.GetCustomAttribute<ReadTypeAttribute>().ReadType, info.GetCustomAttributes(typeof(SmallEndeanAttribute)).FirstOrDefault() != null);
                return result;
            }

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
