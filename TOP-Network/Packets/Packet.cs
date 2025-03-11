using System.Text;

namespace TOP_Network.Packets
{
    public class Packet
    {
        public byte[] Data { get; set; } = new byte[0];
        private Stream? _stream { get; set; }

        public int Size => BitConverter.ToInt32(Data.Take(4).Reverse().ToArray(), 0);


        public int PakcetLenght;

        public Packet() { }
        public Packet(byte[] data) => Init(data);

        public void Init(byte[] data)
        {
            if (data.Length < 10) throw new Exception("Not enough data to be a valid packet");
            Data = data;
            _stream = new MemoryStream(Data, true);
            _stream.Position = 10;
        }

        public Stream GetStream() => _stream ?? throw new Exception("The packet has not been initialized");
        public BinaryReader GetBitReader() => new BinaryReader(GetStream());
        public BinaryWriter GetBitWriter() => new BinaryWriter(GetStream());

        public Packet Clone()
        {
            return new Packet(Data.Take(Size).ToArray());
        }

        public void DisplayHex()
        {
            Console.WriteLine($"{BitConverter.ToString(Data.Take(Size).ToArray()).Replace("-", "")}");
        }
    }
}
