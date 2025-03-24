using NetworkTest.TestClasses;
using System.Text;
using TOP_Network.Attributes;
using TOP_Network.Converter;
using TOP_Network.Exceptions;
using TOP_Records;

namespace NetworkTest.PacketTests
{
    public class ClassToPacketTest
    {
        public Address Addres { get; set; }
        public Painters Painters { get; set; }

        public ClassToPacketTest()
        {
            CentryTable table = new();
            table.Data.Add(item: new Centry { Exist = 1, Index = 1, Value = 16 });
            table.Data.Add(item: new Centry { Exist = 1, Index = 2, Value = 18 });
            RecorReaders.Readers.Add(table);


            Addres = new Address { City = "Antwerp", Code = [(byte)'B', (byte)'E'], Country = "Belgium", PostalCode = 2000 };

            Person Dyck = new() { addres = Addres, Name = "Van Dyck", age = 37, BirthDay = new DateTime(1853, 3, 30, 12, 30, 30), CentryID = 1 };
            Person Elder = new() { addres = Addres, Name = "The Elder", age = 37, BirthDay = new DateTime(1853, 3, 30, 12, 30, 30), CentryID = 2 };
            Person Younger = new() { addres = Addres, Name = "Younger", age = 37, BirthDay = new DateTime(1853, 3, 30, 12, 30, 30), CentryID = 3 };

            Painters = new Painters { People = [Dyck, Elder, Younger] };
        }

        [Fact]
        public void Fact_Address()
        {
            var pkt = Addres.Convert(0).Clone();
            Assert.Equal(36, pkt.Size);
        }

        [Fact]
        public void Fact_Person()
        {
            Person Dyck = Painters.People[0];
            var pkt = Dyck.Convert(0).Clone();
            Assert.Equal(62, pkt.Size);
        }

        [Fact]
        public void Fact_Painters()
        {

            Painters painters = Painters;
            var pkt = painters.Convert(0).Clone();
            pkt.DisplayHex();
            Assert.Equal(120, pkt.Size);
        }
    }
}
