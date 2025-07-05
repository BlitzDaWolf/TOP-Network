using TOP_Network.Attributes;

namespace NetworkTest.TestClasses
{
    public class Person
    {
        [ValidRecord(typeof(CentryTable))]
        public int CentryID { get; set; }
        public string Name { get; set; }
        public Address addres { get; set; }
        public DateTime BirthDay { get; set; }

        public int age { get; set; }
    }

    public class Address
    {
        public string City { get; set; }
        public string Country { get; set; }

        public int PostalCode { get; set; }
        public byte[] Code { get; set; }
    }
}
