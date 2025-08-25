using System;

namespace TOP_Network.Packets.Streams;

public class PacketWriter
{
    private readonly PacketStream Stream;
    public PacketStream BaseStream => Stream;

    public PacketWriter(PacketStream stream)
    {
        Stream = stream;
    }

    public void WriteBytes(byte[] data)
    {
        var sizeNeed = data.Length + Stream.Position;
        if (sizeNeed > Stream.Capacity) throw new Exception("BufferOverflow");

        // while (Stream.Position < sizeNeed)
        for (int i = 0; i < data.Length; i++)
        {
            Stream.Position++;
            Stream.data[Stream.Position] = data[i];
        }
    }

    public void Write(bool value)
    {
        Write((byte)(value ? 1 : 0));
    }

    public void Write(sbyte value)
    {
        // sbyte to byte
        Write((byte)value);
    }

    public void Write(byte data)
    {
        Stream.data[Stream.Position++] = data;
    }
}
