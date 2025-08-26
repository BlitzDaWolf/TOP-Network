using System;

namespace TOP_Network.Interfaces.Packets;

public interface IRPacket : IPacket
{
    public int ReversePoint { get; }

    #region  normal
    public byte ReadChar();
    public int ReadLong();
    public uint ReadULong();

    public short ReadShort();
    public ushort ReadUShort();

    public long ReadLongLong();
    public ulong ReadULongLong();

    public string ReadString();
    public byte[] ReadSeq();
    #endregion

    #region Reverse
    public byte ReverseReadChar();

    public int ReverseReadLong();
    public uint ReverseReadULong();

    public short ReverseReadShort();
    public ushort ReverseReadUShort();

    public long ReverseReadLongLong();
    public ulong ReverseReadULongLong();

    public string ReverseReadString();
    public byte[] ReverseReadSeq();
    #endregion

    public void UnRead(int amount);
}
