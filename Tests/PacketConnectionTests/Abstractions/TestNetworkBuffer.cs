using System;
using TOP_Network.Interfaces;
using TOP_Network.Interfaces.Packets;
using TOP_Network.Packets;

namespace PacketConnectionTests.Abstractions;

public class TestNetworkBuffer : INetworkBuffer
{
    public List<byte> Data { get; private set; } = new List<byte>();

    private int ToRemove = 0;
    public int Remaining => Data.Count - ToRemove;
    public virtual bool EOF => Remaining == 0;

    public void SafeStep()
    {
        lock (Data)
        {
            Data.RemoveRange(0, ToRemove);
        }
        ToRemove = 0;
    }

    public byte[] ReadBuffer(int size)
    {
        if (Remaining < size) throw new Exception("Not enough bites in the buffer");
        var buff = Data.Skip(ToRemove).Take(size).ToArray();
        ToRemove += size;
        return buff;
    }

    public byte[] ReadAll()
    {
        return ReadBuffer(Data.Count - ToRemove);
    }

    public byte[] Peek(int size) => Data.Skip(ToRemove).Take(size).ToArray();

    public virtual IRPacket ReadPacket()
    {
        IRPacket retunValue = new RPacket();
        retunValue.Init([0, 2]);
        return retunValue;
    }

    public void AddData(IEnumerable<byte> data)
    {
        this.Data.AddRange(data);
    }

    public void AddData(IPacket pkt)
    {
        AddData(pkt.GetData());
        pkt.Final();
    }
}
