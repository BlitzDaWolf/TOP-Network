using NetworkTest.TestClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOP_Network.Attributes;
using TOP_Network.Converter;
using TOP_Network.Exceptions;
using TOP_Records;

namespace NetworkTest.Packet
{
    struct test
    {
        [ValidRecord(typeof(CentryTable))]
        public short MyProperty { get; set; }
    }

    struct BaseTest
    {
        public test singleTest { get; set; }
    }

    public class ClassToPacketTest
    {
        public ClassToPacketTest()
        {
            CentryTable table = new CentryTable();
            table.Data.Add(new Centry { Exist = 1, Index = 1, Value = 16 });
            table.Data.Add(new Centry { Exist = 1, Index = 2, Value = 18 });

            RecorReaders.Readers.Add(table);
        }

        [Fact]
        public void Fact_Address()
        {
            Address TestAddress = new Address { City = "Antwerp", Code = [(byte)'B', (byte)'E'], Country = "Belgium", PostalCode = 2000 };
            var pkt = TestAddress.Convert(0).Clone();
            Assert.Equal(36, pkt.Size);
        }

        [Fact]
        public void Fact_Person()
        {
            Address TestAddress = new Address { City = "Antwerp", Code = [(byte)'B', (byte)'E'], Country = "Belgium", PostalCode = 2000 };
            Person Dyck = new Person { addres = TestAddress,    Name = "Van Dyck", age = 37, BirthDay = new DateTime(1853, 3, 30, 12, 30, 30), CentryID = 1 };
            var pkt = Dyck.Convert(0).Clone();
            Assert.Equal(62, pkt.Size);
        }

        [Fact]
        public void Fact_Painters()
        {
            Address TestAddress = new Address { City = "Antwerp", Code = [(byte)'B', (byte)'E'], Country = "Belgium", PostalCode = 2000 };
            Person Dyck = new Person { addres = TestAddress, Name = "Van Dyck", age = 37, BirthDay = new DateTime(1853, 3, 30, 12, 30, 30), CentryID = 1 };
            Person Elder = new Person { addres = TestAddress, Name = "The Elder", age = 37, BirthDay = new DateTime(1853, 3, 30, 12, 30, 30), CentryID = 2 };
            Person Younger = new Person { addres = TestAddress, Name = "Younger", age = 37, BirthDay = new DateTime(1853, 3, 30, 12, 30, 30), CentryID = 3 };

            Painters painters = new Painters { People = [Dyck, Elder, Younger] };
            var pkt = painters.Convert(0).Clone();
            var t = pkt.DisplayHex();
            Assert.Equal(120, pkt.Size);
        }

        [Fact]
        public void Fact_WrongType()
        {
            test t = new test { MyProperty = 5 };
            Assert.Throws<WrongTypeExcetion>(() =>  (new BaseTest { singleTest = t}).Convert(0));
        }
    }
}
