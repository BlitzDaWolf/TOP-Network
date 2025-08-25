using System.Drawing;
using TOP_Network.Enum;
using TOP_Network.Extention;
using TOP_Network.Packets.Streams;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TOP_Network.Packets
{
    public class Packet
    {
        public static bool LongSize = true;
        public static int StartSize => LongSize ? 4 : 2;

        public byte[] Data { get; set; } = new byte[0];
        public bool ValidGnack => gnack == 2147483648;

        /*private Stream? _stream { get; set; }
        private BinaryReader? _reader;
        private BinaryWriter? _writter;*/

        private PacketStream _stream;
        private Streams.PacketReader? _reader;
        private Streams.PacketWriter? _writter;


        public uint gnack => BitConverter.ToUInt32(Data.Skip(StartSize).Take(4).Reverse().ToArray());
        public int Size => (int)(LongSize? BitConverter.ToUInt32(Data.Take(StartSize).Reverse().ToArray(), 0): BitConverter.ToUInt16(Data.Take(StartSize).Reverse().ToArray(), 0));
        public Commands Command => (Commands)BitConverter.ToInt16(Data.Skip(StartSize+4).Take(2).Reverse().ToArray());

        public Packet() { }
        public Packet(byte[] data) => Init(data);

        public void Init(byte[] data)
        {
            // if (data.Length < 6 + StartSize) throw new Exception("Not enough data to be a valid packet");
            Data = data;
            _stream = new PacketStream(Data);
            _stream.Position = 0;
        }

        public PacketStream GetStream() => _stream;
        public Streams.PacketReader GetBitReader()
        {
            if (_reader == null) _reader = new Streams.PacketReader(_stream);
            return _reader;
        }
        public PacketWriter GetBitWriter()
        {
            if (_writter == null) _writter = new PacketWriter(_stream);
            return _writter;
        }

        public void WriteSize(int size)
        {
            var writer = GetBitWriter();
            var currentPos = writer.BaseStream.Position;

            byte[] data = [];

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

        public virtual void RemoveLast(int size)
        {
            for (int i = this.Size - size; i < this.Size; i++)
            {
                Data[i] = 0;
            }
            Data =Data.Take(this.Size - size).ToArray();
            WriteSize(this.Size - size);
        }

        public string DisplayHex() => /*Display(); =>*/ $"{BitConverter.ToString(Data.Take(Size).ToArray()).Replace("-", " ")}";

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
            if (_stream != null) _stream.Close();

            _reader = null;
            _writter = null;
            _stream = null;
        }

        public byte[] GetData() => Data.Take(Size).ToArray();

        public byte[] GetGnack() => Data.Skip(StartSize).Take(4).Reverse().ToArray();
    }
}
