using System.Text;

namespace TOP_Network.Packets
{
    public class Packet
    {
        public byte[] Data { get; set; }
        private Stream? _stream { get; set; }


        public int PakcetLenght;

        public Packet()
        {
            Data = new byte[0];
        }

        public void Init(byte[] data)
        {
            Data = data;
            _stream = new MemoryStream(Data, true);
        }

        public Stream GetStream() => _stream ?? throw new Exception("The packet has not been initialized");
        public BinaryReader GetBitReader() => new BinaryReader(GetStream());
        public BinaryWriter GetBitWriter() => new BinaryWriter(GetStream());
    }
}
