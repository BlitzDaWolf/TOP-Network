using System;
using TOP_Network.Interfaces;
using TOP_Network.Interfaces.Packets;
using TOP_Network.Packets;
using TOP_Utils;

namespace TOP_Network;

public class NetworkBuffer : INetworkBuffer
{
    public List<byte> Data { get; private set; } = new List<byte>();


    private int ToRemove = 0;
    public int Remaining => Data.Count - ToRemove;
    public bool EOF => Remaining == 0;

    public void SafeStep()
    {
        using var stepActiveit = this.StartActivity("safeStep");
        stepActiveit?.SetTag("remove", ToRemove);
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

    public IRPacket ReadPacket()
    {
        var p = Peek(2);// Dynamic sizing
        if (p.Length < 2) throw new Exception("Not packet size");
        IPacket sizePacket = new Packet{ LongSize = false};
        sizePacket.Init(p);
        if (sizePacket.Size > Remaining) throw new Exception("Packet in in buffer");
        IRPacket retunValue = new RPacket{LongSize = false};
        retunValue.Init(ReadBuffer(sizePacket.Size));
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
