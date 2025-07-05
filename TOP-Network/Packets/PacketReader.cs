namespace TOP_Network.Packets
{
    public class PacketReader : BinaryReader
    {
        public static Action<byte[]> OnRead;

        public PacketReader(Stream input) : base(input) { }

        public override byte[] ReadBytes(int size)
        {
            var bytes = base.ReadBytes(size);

            if(OnRead != null)
            {
                OnRead(bytes);
            }

            return bytes;
        }

        public override byte ReadByte()
        {
            return ReadBytes(1)[0];
        }
    }
}
