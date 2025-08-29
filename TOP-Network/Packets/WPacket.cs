using System.Net;
using TOP_Network;
using TOP_Network.Extention;
using TOP_Network.Interfaces.Packets;

namespace TOP_Network.Packets;

public class WPacket : Packet, IWPacket
{
    public WPacket() : base()
    {
        Init(new byte[32_768]);
        WriteSize(StartSize + 4 + 2);
        GetStream().Position = Size;
    }

    public WPacket(IPacket packet) : this()
    {
        GetStream().Position = 0;
        GetWriter().WriteBytes(packet.Data);
    }

    public override void Init(byte[] data)
    {
        if (data.Length > Data.Length)
            base.Init(data);
        GetStream().Position = 0;
        GetWriter().WriteBytes(data);
        GetStream().Position = Size;
    }

    public bool WriteChar(byte value)
    {
        GetWriter().WriteType(value);
        WriteSize(GetStream().Position);
        return true;
    }

    public bool WriteLong(int value)
    {
        GetWriter().WriteType(value);
        WriteSize(GetStream().Position);
        return true;
    }

    public bool WriteLongLong(long value)
    {
        GetWriter().WriteType(value);
        WriteSize(GetStream().Position);
        return true;
    }

    public bool WriteSeq(byte[] value)
    {
        GetWriter().WriteType(value);
        WriteSize(GetStream().Position);
        return true;
    }

    public bool WriteShort(short value)
    {
        GetWriter().WriteType(value);
        WriteSize(GetStream().Position);
        return true;
    }

    public bool WriteString(string value)
    {
        if (value.Last() != '\0') value += "\0";

        GetWriter().WriteType(value);
        WriteSize(GetStream().Position);
        return true;
    }

    public bool WriteULong(uint value)
    {
        GetWriter().WriteType(value);
        WriteSize(GetStream().Position);
        return true;
    }

    public bool WriteULongLong(ulong value)
    {
        GetWriter().WriteType(value);
        WriteSize(GetStream().Position);
        return true;
    }

    public bool WriteUShort(ushort value)
    {
        GetWriter().WriteType(value);
        WriteSize(GetStream().Position);
        return true;
    }
}

