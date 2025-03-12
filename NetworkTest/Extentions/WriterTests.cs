using TOP_Network.Exceptions;
using TOP_Network.Extention;

namespace NetworkTest.Extentions
{
    public class WriterTests
    {
        public const int TestAmount = 100;

        public BinaryWriter CreateWriter(byte[] Data)
        {
            /*if(!Directory.Exists("test"))
                Directory.CreateDirectory("test");
            var name = $"t-{DateTime.Now.Ticks}.tst";
            if(!File.Exists(@$"test\{name}"))
                File.Create($@"test\{name}").Close();
            var stream = File.OpenWrite(@$"test\{name}");*/
            var stream = new MemoryStream(Data, true);
            return new BinaryWriter(stream);
        }

        public byte[] RandomType<T>()
        {
            byte[] res = Enumerable.Range(0, typeof(T).SizeOf()).Select(x => (byte)Random.Shared.Next(0, 255)).ToArray();
            return res;
        }

        [Fact]
        public void Fact_WriteUInt()
        {
            for (int i = 0; i < TestAmount; i++)
            {
                byte[] target = RandomType<uint>();
                byte[] Data = new byte[target.Length];
                using var writer = CreateWriter(Data);

                writer.WriteType(BitConverter.ToUInt32(target.Reverse().ToArray()));
                Assert.Equal(target, Data);
            }
        }

        [Fact]
        public void Fact_WriteInt()
        {
            for (int i = 0; i < TestAmount; i++)
            {
                byte[] target = RandomType<int>();
                byte[] Data = new byte[target.Length];
                using var writer = CreateWriter(Data);

                writer.WriteType(BitConverter.ToInt32(target.Reverse().ToArray()));
                Assert.Equal(target, Data);
            }
        }

        [Fact]
        public void Fact_WriteULong()
        {
            for (int i = 0; i < TestAmount; i++)
            {
                byte[] target = RandomType<ulong>();
                byte[] Data = new byte[target.Length];
                using var writer = CreateWriter(Data);

                writer.WriteType(BitConverter.ToUInt64(target.Reverse().ToArray()));
                Assert.Equal(target, Data);
            }
        }

        [Fact]
        public void Fact_WriteLong()
        {
            for (int i = 0; i < TestAmount; i++)
            {
                byte[] target = RandomType<long>();
                byte[] Data = new byte[target.Length];
                using var writer = CreateWriter(Data);

                writer.WriteType(BitConverter.ToInt64(target.Reverse().ToArray()));
                Assert.Equal(target, Data);
            }
        }

        [Fact]
        public void Fact_WriteUShort()
        {
            for (int i = 0; i < TestAmount; i++)
            {
                byte[] target = RandomType<ushort>();
                byte[] Data = new byte[target.Length];
                using var writer = CreateWriter(Data);

                writer.WriteType(BitConverter.ToUInt16(target.Reverse().ToArray()));
                Assert.Equal(target, Data);
            }
        }

        [Fact]
        public void Fact_WriteShort()
        {
            for (int i = 0; i < TestAmount; i++)
            {
                byte[] target = RandomType<short>();
                byte[] Data = new byte[target.Length];
                using var writer = CreateWriter(Data);

                writer.WriteType(BitConverter.ToInt16(target.Reverse().ToArray()));
                Assert.Equal(target, Data);
            }
        }

        [Fact]
        public void Fact_WriteSByte()
        {
            for (int i = 0; i < TestAmount; i++)
            {
                sbyte target = (sbyte)(byte)Random.Shared.Next(0, 255);
                byte[] Data = new byte[1];
                using var writer = CreateWriter(Data);

                writer.WriteType(target);
                Assert.Equal([(byte)target], Data);
            }
        }

        [Fact]
        public void Fact_WriteByte()
        {
            for (int i = 0; i < TestAmount; i++)
            {
                byte target = (byte)Random.Shared.Next(0, 255);
                byte[] Data = new byte[1];
                using var writer = CreateWriter(Data);

                writer.WriteType(target);
                Assert.Equal([target], Data);
            }
        }

        [Fact]
        public void Fact_WriteString()
        {
            var v = "Test";
            byte[] Data = new byte[v.Length + 2];
            using var writer = CreateWriter(Data);
            writer.WriteType(v);
            Assert.Equal([0x00, 0x04, 0x54, 0x65, 0x73, 0x74], Data);
        }

        [Fact]
        public void Fact_WriteBool()
        {
            {
                var v = false;
                byte[] Data = new byte[1];
                using var writer = CreateWriter(Data);
                writer.WriteType(v);
                Assert.Equal([0x00], Data);
            }
            {
                var v = true;
                byte[] Data = new byte[1];
                using var writer = CreateWriter(Data);
                writer.WriteType(v);
                Assert.Equal([0x01], Data);
            }
        }


        [Fact]
        public void Fact_WriteDate()
        {
            var v = new DateTime(2025, 8, 4, 14, 30, 20);
            byte[] Data = new byte[8];
            using var writer = CreateWriter(Data);
            writer.WriteType(v);
            Assert.Equal([0x08, 0xDD, 0xD3, 0x63, 0x70, 0x8D, 0x26, 0x00], Data);
        }



        [Fact]
        public void Fact_WriteFloat()
        {
            for (int i = 0; i < TestAmount; i++)
            {
                byte[] target = RandomType<float>();
                byte[] Data = new byte[target.Length];
                using var writer = CreateWriter(Data);

                writer.WriteType(BitConverter.ToSingle(target.Reverse().ToArray()));
                Assert.Equal(target, Data);
            }
        }

        [Fact]
        public void Fact_WriteDouble()
        {
            for (int i = 0; i < TestAmount; i++)
            {
                byte[] target = RandomType<double>();
                byte[] Data = new byte[target.Length];
                using var writer = CreateWriter(Data);

                writer.WriteType(BitConverter.ToDouble(target.Reverse().ToArray()));
                Assert.Equal(target, Data);
            }
        }

        [Fact]
        public void Fact_Break()
        {
            byte[] Data = new byte[16];
            using var writer = CreateWriter(Data);

            Assert.Throws<BinaryException>(() => writer.WriteType(new List<object>()));
        }
    }
}
