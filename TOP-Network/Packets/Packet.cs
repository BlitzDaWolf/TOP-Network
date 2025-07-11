using System.Text;
using TOP_Network.Enum;
using TOP_Network.Extention;

namespace TOP_Network.Packets
{
    public class Packet
    {
        public static bool LongSize = true;
        public static int StartSize => LongSize ? 4 : 2;

        public byte[] Data { get; set; } = new byte[0];
        public bool ValidGnack => gnack == 128;
        private Stream? _stream { get; set; }

        public uint gnack => BitConverter.ToUInt32(Data.Skip(StartSize).Take(4).Reverse().ToArray());
        public int Size => LongSize? BitConverter.ToInt32(Data.Take(StartSize).Reverse().ToArray(), 0): BitConverter.ToInt16(Data.Take(StartSize).Reverse().ToArray(), 0);
        public Commands Command => (Commands)BitConverter.ToInt16(Data.Skip(StartSize+4).Take(2).Reverse().ToArray());

        public Packet() { }
        public Packet(byte[] data) => Init(data);

        public void Init(byte[] data)
        {
            // if (data.Length < 6 + StartSize) throw new Exception("Not enough data to be a valid packet");
            Data = data;
            _stream = new MemoryStream(Data, true);
            _stream.Position = 0;
        }

        public Stream GetStream() => _stream ?? throw new Exception("The packet has not been initialized");
        public BinaryReader GetBitReader() => new PacketReader(GetStream());
        public BinaryWriter GetBitWriter() => new BinaryWriter(GetStream());

        public void WriteSize(int size)
        {
            using var writer = GetBitWriter();
            var currentPos = writer.BaseStream.Position;

            writer.BaseStream.Position = 0;
            if (LongSize)
            {
                writer.WriteType(size);
            }
            else
            {
                writer.WriteType((short)size);
            }
            // writer.WriteType(LongSize ? size : (short)size);

            writer.BaseStream.Position = currentPos;
        }

        public void WriteNewGnack(uint gnac)
        {
            using var reader = GetBitWriter();
            var currentPos = reader.BaseStream.Position;
            reader.BaseStream.Position = StartSize;
            reader.WriteType(gnac);
            reader.BaseStream.Position = currentPos;
        }

        public Packet Clone()
        {
            return new Packet(Data.Take(Size).ToArray());
        }

        public void RemoveLast(int size)
        {
            WriteSize(this.Size - size);
        }

        public string DisplayHex() => $"{BitConverter.ToString(Data.Take(Size).ToArray()).Replace("-", " ")}";

        public void Save(string Path)
        {
            File.WriteAllBytes(Path, Data);
        }

        public void AddRandomGnack()
        {
            var rnd = new byte[2];
            Random.Shared.NextBytes(rnd);
            Data[StartSize + 3] = rnd[0];
            Data[StartSize + 2] = rnd[1];
        }

        public int ReadPlayer()
        {
            var v = Data.TakeLast(4).Reverse().ToArray();
            return BitConverter.ToInt32(v);
        }
    }
}
