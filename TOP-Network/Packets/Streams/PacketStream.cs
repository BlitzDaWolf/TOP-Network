using System;

namespace TOP_Network.Packets.Streams;

public class PacketStream
{
    public int Position { get; set; }
    public int Capacity => data.Length;
    public bool EOF => Position == Capacity;
    public byte[] data;

    public PacketStream(byte[] data)
    {
        this.data = data;
    }

    internal void Close()
    {
        Position = 0;
        data = Array.Empty<byte>();
    }
}
