using System.Drawing;
using TOP_Network.Enum;
using TOP_Network.Extention;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public int Size => (int)(LongSize? BitConverter.ToUInt32(Data.Take(StartSize).Reverse().ToArray(), 0): BitConverter.ToUInt16(Data.Take(StartSize).Reverse().ToArray(), 0));
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

        private BinaryReader _reader;
        private BinaryWriter _writter;

        public BinaryReader GetBitReader()
        {
            if (_reader==null) _reader = new PacketReader(GetStream());
            if (!_stream.CanWrite)
            {
                _stream = new MemoryStream(Data, true);
                _stream.Position = Size;
                _reader = new PacketReader(GetStream());
            }
            return _reader;
        }
        public BinaryWriter GetBitWriter() {
            if(_writter==null) _writter = new BinaryWriter(GetStream());
            if (!_stream.CanWrite)
            {
                _stream = new MemoryStream(Data, true);
                _stream.Position = Size;
                _writter = new BinaryWriter(_stream);
            }
            return _writter;
        }

        // public BinaryReader GetBitReader() => new PacketReader(GetStream());
        // public BinaryWriter GetBitWriter() => new BinaryWriter(GetStream());

        public void WriteSize(int size)
        {
            using var writer = GetBitWriter();
            var currentPos = writer.BaseStream.Position;

            byte[] data = new byte[0];

            // writer.BaseStream.Position = 0;
            if (LongSize)
            {
                data = BitConverter.GetBytes(size);
            }
            else
            {
                data = BitConverter.GetBytes((short)size);
            }

            data = data.Reverse().ToArray();
            for (int i = 0; i < data.Length; i++) this.Data[i] = data[i];
            // writer.WriteType(LongSize ? size : (short)size);

            Logging.LogInfo(Command);
            writer.BaseStream.Position = size;
        }

        public void WriteNewGnack(uint gnac)
        {
            var data = BitConverter.GetBytes(gnac).Reverse().ToArray();
            for (int i = 0; i < data.Length; i++) this.Data[StartSize+ i] = data[i];
        }

        public void WriteCMD(Commands command)
        {
            var data = BitConverter.GetBytes((short)command).Reverse().ToArray();
            for (int i = 0; i < data.Length; i++) this.Data[StartSize + i+4] = data[i];
        }

        public virtual Packet Clone()
        {
            return new Packet(Data.Take(Size).ToArray());
        }

        public void RemoveLast(int size)
        {
            for (int i = this.Size - size; i < this.Size; i++)
            {
                Data[i] = 0;
            }
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

        public void Final()
        {
            if (_reader != null) _reader.Close();
            if (_writter != null) _writter.Close();
            if (_stream != null) _stream.Close();

            _reader = null;
            _writter = null;
            _stream = null;
        }
    }
}
