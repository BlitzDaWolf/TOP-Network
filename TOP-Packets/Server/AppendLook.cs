using TOP_Network.Attributes;

namespace TOP_Packets.Server
{
    public class Look
    {
        public ushort LookID { get; set; }
        [NotIf("LookID", (ushort)0)]
        public bool Valid { get; set; }
    }

    public class AppendLook
    {
        public uint EntityID { get; set; }
        [ArrayLength(4)]
        public Look[] Look { get; set; }
    }
}
