using TOP_Network.Enum;
using TOP_Network.Interfaces.Packets;
using TOP_Network.Packets.Streams;

namespace TOP_Network.Packets;

public class Packet : IPacket
{
    public bool LongSize { get; set; } = false;
    public int StartSize => LongSize ? 4 : 2;

    public byte[] Data { get; private set; } = Array.Empty<byte>();

    public bool ValidGnack => GNACK == 2147483648;

    public PacketStream Stream { get; private set; }

    public PacketReader? Reader { get; private set; }
    public PacketWriter? Writer { get; private set; }

    public uint GNACK => BitConverter.ToUInt32(Data.Skip(StartSize).Take(4).Reverse().ToArray());
    public int Size => (int)(LongSize ? BitConverter.ToUInt32(Data.Take(StartSize).Reverse().ToArray(), 0) : BitConverter.ToUInt16(Data.Take(StartSize).Reverse().ToArray(), 0));
    public Commands Command => (Commands)BitConverter.ToInt16(Data.Skip(StartSize + 4).Take(2).Reverse().ToArray());

    public IPacket Clone<T>() where T : IPacket, new()
    {
        var pkt = new T();

        pkt.Init(Data);
        pkt.LongSize = LongSize;

        return pkt;
    }


    public PacketStream GetStream() => Stream;
    public PacketReader GetReader()
    {
        if (Reader == null) Reader = new PacketReader(Stream);
        return Reader;
    }
    public PacketWriter GetWriter()
    {
        if (Writer == null) Writer = new PacketWriter(Stream);
        return Writer;
    }

    public virtual void Init(byte[] data)
    {
        Data = data;
        Stream = new PacketStream(Data);
        Stream.Position = 4 + StartSize;
    }

    public void Remove(int amount)
    {
        // Stream.Position -= amount;
        for (int i = Stream.Position; i < Data.Length - amount; i++)
        {
            Data[i] = Data[i + amount];
        }
        WriteSize(Size - amount);
        Data = Data.Take(Size).ToArray();
    }

    public virtual void RemoveLast(int amount)
    {
        WriteSize(Size - amount);
        Data = Data.Take(Size).ToArray();
    }

    public void WriteCommand(Commands commands)
    {
        byte[] data = BitConverter.GetBytes((short)commands).Reverse().ToArray();
        for (int i = 0; i < data.Length; i++) this.Data[i + StartSize + 4] = data[i];
    }

    public void WriteGnack(uint NewGNACK)
    {
        byte[] data = BitConverter.GetBytes(NewGNACK);
        for (int i = 0; i < data.Length; i++) this.Data[i + StartSize] = data[i];
    }

    public void WriteSize(int newSize)
    {
        byte[] data;

        if (LongSize) data = BitConverter.GetBytes(newSize);
        else data = BitConverter.GetBytes((short)newSize);

        data = data.Reverse().ToArray();

        for (int i = 0; i < data.Length; i++) this.Data[i] = data[i];
    }

    public byte[] GetData() => Data.Take(Size).ToArray();
    
    public void Final()
    {
        if (Stream != null) Stream.Close();

        Reader = null;
        Writer = null;
        Stream = null;
    }
}
