using TOP_Network.Attributes;

namespace TOP_Packets.Server
{
    public class LoginAccount
    {
        [BreakIf(false)]
        public bool Valid { get; set; }

        public uint ID { get; set; }
        public byte Slot { get; set; }
        public string Name { get; set; }
        public string Job { get; set; }
        public short Level { get; set; }
        public byte[] Look { get; set; }
    }

    public class LoginResponse
    {
        public short EnterError { get; set; }
        [NotIf("EnterError", (short)0)]
        [EndIf]
        public string Reason { get; set; }

        public byte[] ChatKey { get; set; }

        public LoginAccount[] Accounts { get; set; }
        
        [SmallEndean]
        public int Password { get; set; }
        public int CommunicationEncryption { get; set; }
        public int Flag { get; set; }
    }
}
