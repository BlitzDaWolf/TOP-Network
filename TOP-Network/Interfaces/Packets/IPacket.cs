using TOP_Network.Enum;
using TOP_Network.Packets.Streams;

namespace TOP_Network.Interfaces.Packets;

public interface IPacket
{
    public bool LongSize { get; set; }
    public int StartSize { get; }

    public byte[] Data { get; }
    public bool ValidGnack { get; }

    public PacketStream Stream { get; }

    public PacketReader? Reader { get; }
    public PacketWriter? Writer { get; }

    public uint GNACK { get; }
    public int Size { get; }
    public Commands Command { get; }

    public void Init(byte[] data);

    public PacketStream GetStream();
    public PacketReader GetReader();
    public PacketWriter GetWriter();

    public IPacket Clone<T>() where T : IPacket, new();
    public void RemoveLast(int amount);
    public void Remove(int amount);

    public void WriteSize(int newSize);
    public void WriteGnack(uint NewGNACK);
    public void WriteCommand(Commands commands);

    public byte[] GetData();
}
