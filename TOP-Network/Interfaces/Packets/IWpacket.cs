using System;

namespace TOP_Network.Interfaces.Packets;

public interface IWPacket : IPacket
{
    public bool WriteChar(byte value);
    public bool WriteLong(int value);
    public bool WriteULong(uint value);

    public bool WriteShort(short value);
    public bool WriteUShort(ushort value);

    public bool WriteLongLong(long value);
    public bool WriteULongLong(ulong value);

    public bool WriteString(string value);
    public bool WriteSeq(byte[] value);
}
