using TOP_Network.Attributes;
using TOP_Records.Tables;

namespace Test
{
    public class LoginAccount
    {
        public bool Valid { get; set; }
        public uint ID { get; set; }
        public byte Slot { get; set; }
        public string Name { get; set; }
        public string Job { get; set; }
        public ushort Level { get; set; }

        public byte[] Look { get; set; }
    }

    public class Login
    {
        public short EnterMap { get; set; }
        public byte[] ChatKey { get; set; }

        public LoginAccount[] Accounts { get; set; }


        public int Password { get; set; }
        public int CommEncryption { get; set; }
        public int Flag { get; set; }
    }
}
