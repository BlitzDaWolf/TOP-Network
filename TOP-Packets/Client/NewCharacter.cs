using static TOP_Packets.Server.LoginAccount;

namespace TOP_Packets.Client
{
    public class NewCharacter
    {
        public string Name { get; set; }
        public string A { get; set; }
        public string BirthPlace { get; set; }

        public short Size { get; set; } = 1826;
        public Look _Look { get; set; }

        // public byte[] Look { get; set; }
    }
}
