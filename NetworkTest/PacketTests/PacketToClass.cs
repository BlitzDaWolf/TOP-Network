using NetworkTest.TestClasses;
using TOP_Network.Converter;
using TOP_Network.Packets;
using TOP_Records;

namespace NetworkTest.PacketTests
{
    public class PacketToClass
    {
        public Address Addres { get; set; }
        public Painters Painters { get; set; }

        public PacketToClass()
        {
            CentryTable table = new CentryTable();
            table.Data.Add(new Centry { Exist = 1, Index = 1, Value = 16 });
            table.Data.Add(new Centry { Exist = 1, Index = 2, Value = 18 });
            RecorReaders.Readers.Add(table);


            Addres = new Address { City = "Antwerp", Code = [(byte)'B', (byte)'E'], Country = "Belgium", PostalCode = 2000 };

            Person Dyck = new Person { addres = Addres, Name = "Van Dyck", age = 37, BirthDay = new DateTime(1853, 3, 30, 12, 30, 30), CentryID = 1 };
            Person Elder = new Person { addres = Addres, Name = "The Elder", age = 37, BirthDay = new DateTime(1853, 3, 30, 12, 30, 30), CentryID = 2 };
            Person Younger = new Person { addres = Addres, Name = "Younger", age = 37, BirthDay = new DateTime(1853, 3, 30, 12, 30, 30), CentryID = 3 };

            Painters = new Painters { People = [Dyck, Elder, Younger] };
        }

        [Fact]
        public void Fact_Address()
        {
            Packet pkt = new Packet(File.ReadAllBytes("./test/address.tst"));
            var adr = pkt.Convert<Address>();
            Assert.Equal(Addres.City, adr.City);
            Assert.Equal(Addres.Code, adr.Code);
            Assert.Equal(Addres.Country, adr.Country);
            Assert.Equal(Addres.PostalCode, adr.PostalCode);
        }

        [Fact]
        public void Fact_Person()
        {
            Packet pkt = new Packet(File.ReadAllBytes("./test/person.tst"));
            var person = pkt.Convert<Person>();
            Assert.Equal(Painters.People[0].Name, person.Name);
            Assert.Equal(Painters.People[0].age, person.age);
            Assert.Equal(Painters.People[0].BirthDay, person.BirthDay);
            Assert.Equal(Painters.People[0].CentryID, person.CentryID);

            var adr = person.addres;
            Assert.Equal(Addres.City, adr.City);
            Assert.Equal(Addres.Code, adr.Code);
            Assert.Equal(Addres.Country, adr.Country);
            Assert.Equal(Addres.PostalCode, adr.PostalCode);
        }

        /*[Fact]
        public void Fact_Painters()
        {
            Packet pkt = new Packet(File.ReadAllBytes("./test/list.tst"));
            var painters = pkt.Convert<Painters>();

            Assert.Equal(3, painters.People.Length);
            Assert.NotNull(painters.People[0]);
            Assert.Null(painters.People[2]);
        }*/
    }
}
