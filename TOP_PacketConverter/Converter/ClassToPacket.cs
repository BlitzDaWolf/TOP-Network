using System.Reflection;
using System.Reflection.PortableExecutable;
using TOP_PacketConverter.Attributes;
using TOP_PacketConverter.Enum;
using TOP_PacketConverter.Exceptions;
using TOP_PacketConverter.Extention;
using TOP_PacketConverter.Packets;
using TOP_Records;

namespace TOP_PacketConverter.Converter
{
    public static class ClassToPacket
    {

        public static Packet Convert(this object entity, Commands command)
        {
            Dictionary<PropertyInfo, object> values = [];

            Packet packet = new();
            packet.Init(new byte[4096*4]);
            using BinaryWriter writer = packet.GetBitWriter();
            writer.BaseStream.Position = 0;

            if (Packet.LongSize)
            {
                writer.WriteType(50);
            }
            else
            {
                writer.WriteType((short)50);
            }
            writer.WriteType(-2147483648);
            writer.WriteType((short)command);

            writer.WriteData(entity, values);

            var size = (int)packet.GetStream().Position;
            packet.GetStream().Position = 0;

            if (Packet.LongSize)
            {
                writer.WriteType(size);
            }
            else
            {
                writer.WriteType((short)size);
            }

            return packet.Clone();
        }

        public static Packet Convert(this object entity)
        {
            var c = entity.GetType().GetCustomAttribute<DefaultCommandAttribute>();
            if (c == null) throw new Exception("No default command has been found");

            return Convert(entity, c.Command);
        }

        private static void WriteData(this BinaryWriter writer, object entity, Dictionary<PropertyInfo, object> values)
        {
            Dictionary<PropertyInfo, object> test = new Dictionary<PropertyInfo, object>(values);
            var properties = entity.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                // Valid
                if (Valid(property, entity))
                {
                    test.Add(property, property.GetValue(entity)!);
                    writer.WriteSingle(property, test);
                    return;
                }

                // BreakIf
                if(Break(property, entity))
                {
                    test.Add(property, property.GetValue(entity)!);
                    writer.WriteSingle(property, test);
                    return;
                }

                // Chooise
                var ChooseResult = Choise(property, entity);
                if(ChooseResult != null)
                {
                    writer.WriteType(ChooseResult, ChooseResult.ReadType);
                    return;
                }

                // ReadIf
                var ifResult = If(property, entity, test);
                if (ifResult != null)
                {
                    if (ifResult.Value)
                    {
                        continue;
                    }
                    else
                    {

                    }
                }

                // Read
                writer.Write(property, entity, test);
            }
        }

        private static void Write(this BinaryWriter writer, PropertyInfo info, object entity, Dictionary<PropertyInfo, object> values)
        {
            values.Add(info, info.GetValue(entity)!);
            if (info.PropertyType.IsArray)
            {
                writer.WriteArry(info, values);
            }
            else if (info.PropertyType.IsEnum)
            {
                var att = info.GetCustomAttribute<ReadTypeAttribute>()!;
                writer.WriteLength(att.ReadType, values[info].GetHashCode());
            }
            else
            {
                writer.WriteSingle(info, values);
            }
        }

        private static bool? If(PropertyInfo info, object entity, Dictionary<PropertyInfo, object> values)
        {
            List<IfAttribute> ifs = info.GetCustomAttributes<IfAttribute>().Where(x => x is not WhileAttribute).ToList();
            if (ifs.Count == 0) return null;

            var vals = ifs.FirstOrDefault(x => x.A(values.FirstOrDefault(y => y.Key.Name == x.v1).Value));

            return vals == null;
        }

        private static bool Valid(PropertyInfo info, object entity)
        {
            var validAttibutes = info.GetCustomAttribute<ValidRecordAttribute>();
            if (validAttibutes == null) return false;

            var id = info.GetValue(entity)!.GetHashCode();
            return RecorReaders.GetRecord(validAttibutes.RecoredTable, id) != null ? false : true;
        }


        private static bool Break(PropertyInfo info, object entity)
        {
            var breakAttibutes = info.GetCustomAttributes<BreakIfAttribute>().ToList();
            if (breakAttibutes.Count == 0) return false;

            var r = info.GetValue(entity);
            return breakAttibutes.Where(x => x.Check(r)).Count() != 0;
        }

        private static ChooseAttribute? Choise(PropertyInfo info, object entity)
        {
            var ChooseAttibutes = info.GetCustomAttributes<ChooseAttribute>().ToList();
            if (ChooseAttibutes.Count == 0) return null;

            var o = info.GetValue(entity);
            var c = ChooseAttibutes.FirstOrDefault(x => x.DataType == o.GetType());
            return c;
        }

        private static void WriteSingle(this BinaryWriter writer, PropertyInfo info, Dictionary<PropertyInfo, object> values)
        {
            if (!writer.WriteType(values[info], info.GetCustomAttributes(typeof(SmallEndeanAttribute)).FirstOrDefault() != null))
            {
                writer.WriteData(values[info], values);
            }
        }

        public static void WriteArry(this BinaryWriter writer, PropertyInfo info, Dictionary<PropertyInfo, object> values)
        {
            if (!writer.WriteType(values[info]))
            {
                var arr = (Array)values[info];
                var ArrayType = info.GetCustomAttribute<ArraySizeAttribute>();
                var ArraySize = info.GetCustomAttribute<ArrayLengthAttribute>();
                var While = info.GetCustomAttribute<WhileAttribute>();

                if (ArrayType != null)
                {
                    writer.WriteLength(ArrayType.ReadType, arr.Length);
                }
                else if (ArraySize != null)
                {

                }
                else
                {
                    writer.WriteLength(typeof(byte), arr.Length);
                }

                bool writeFalse = false;

                for (int i = 0; i < arr.Length; i++)
                {
                    if (While != null)
                    {
                        var v = arr.GetValue(i);
                        if (v == null)
                        {
                            writeFalse = true;
                        }
                        else
                        {
                            writer.WriteLength(While.Target.GetType(), i);
                            writer.WriteData(v, values);
                        }
                    }
                    else
                    {
                        var t = arr.GetValue(i);
                        if (t.GetType().GetProperties().Length == 0)
                        {
                            writer.WriteType(arr.GetValue(i), info.GetCustomAttributes(typeof(SmallEndeanAttribute)).FirstOrDefault() != null);
                            // writer.WriteSingle(info, values);
                        }
                        else
                        {
                            writer.WriteData(arr.GetValue(i)!, values);
                        }
                    }
                }
                if (writeFalse)
                {
                    writer.WriteType(While.Target);
                }
            }
        }

        private static void WriteLength(this BinaryWriter writer, Type writeType, int size)
        {
            object t = null;

            if (writeType == typeof(int)) t = size;
            if (writeType == typeof(uint)) t = (uint)size;

            if (writeType == typeof(ushort)) t = (ushort)size;
            if (writeType == typeof(short)) t = (short)size;

            if (writeType == typeof(sbyte)) t = (sbyte)size;
            if (writeType == typeof(byte)) t = (byte)size;
            
            writer.WriteType(t);
        }
    }
}
