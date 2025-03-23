using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOP_Network.Extention;

namespace NetworkTest.Extentions
{
    public class ReaderTests
    {
        public BinaryReader CreateReader(string path)
        {
            var stream = File.OpenRead(Path.Combine("test", path));
            return new BinaryReader(stream);
        }

        [Fact]
        public void Fact_ReadBool()
        {
            using var reader = CreateReader("bool.tst");
            Assert.True(reader.ReadType<bool>());
        }
        [Fact]
        public void Fact_ReadByte()
        {
            using var reader = CreateReader("byte.tst");
            Assert.Equal(64, reader.ReadType<byte>());
        }
        [Fact]
        public void Fact_ReadDate()
        {
            using var reader = CreateReader("date.tst");
            Assert.Equal(new DateTime(2025, 8, 4, 14, 30, 20), reader.ReadType<DateTime>());
        }
        [Fact]
        public void Fact_ReadDouble()
        {
            using var reader = CreateReader("double.tst");
            Assert.Equal(-580.745, reader.ReadType<double>());
        }
        [Fact]
        public void Fact_ReadFloat()
        {
            using var reader = CreateReader("float.tst");
            Assert.Equal(-1810443, reader.ReadType<float>());
        }
        [Fact]
        public void Fact_ReadInt()
        {
            using var reader = CreateReader("int.tst");
            Assert.Equal(-502909802, reader.ReadType<int>());
        }
        [Fact]
        public void Fact_ReadLong()
        {
            using var reader = CreateReader("long.tst");
            Assert.Equal(7584452608926539915, reader.ReadType<long>());
        }
        [Fact]
        public void Fact_ReadSByte()
        {
            using var reader = CreateReader("sbyte.tst");
            Assert.Equal(-105, reader.ReadType<sbyte>());
        }
        [Fact]
        public void Fact_Readshort()
        {
            using var reader = CreateReader("short.tst");
            Assert.Equal(7113, reader.ReadType<short>());
        }
        [Fact]
        public void Fact_ReadString()
        {
            using var reader = CreateReader("string.tst");
            Assert.Equal("Test", reader.ReadType<string>());
        }
        [Fact]
        public void Fact_ReadBytes()
        {
            using var reader = CreateReader("string.tst");
            Assert.Equal("Test".Select(x => (byte)x).ToArray(), reader.ReadType<byte[]>());
        }

        [Fact]
        public void Fact_ReadUInt()
        {
            using var reader = CreateReader("uint.tst");
            Assert.Equal((uint)2181423781, reader.ReadType<uint>());
        }
        [Fact]
        public void Fact_ReadULong()
        {
            using var reader = CreateReader("ulong.tst");
            Assert.Equal((ulong)12506315698059693952, reader.ReadType<ulong>());
        }
        [Fact]
        public void Fact_ReadUShort()
        {
            using var reader = CreateReader("ushort.tst");
            Assert.Equal((ushort)17339, reader.ReadType<ushort>());
        }
    }
}
