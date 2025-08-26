using System;
using TOP_Network.Interfaces.Packets;
using TOP_Network.Packets;
using TOP_Utils;

namespace TOP_Network;

public class NetworkBuffer
{
    List<byte> data = new List<byte>();


    private int ToRemove = 0;
    public int Remaining => data.Count - ToRemove;
    public bool EOF => Remaining == 0;

    public void SafeStep()
    {
        using var stepActiveit = this.StartActivity("safeStep");
        stepActiveit?.SetTag("remove", ToRemove);
        lock (data)
        {
            data.RemoveRange(0, ToRemove);
        }
        ToRemove = 0;
    }

    public byte[] ReadBuffer(int size)
    {
        if (Remaining < size) throw new Exception("Not enough bites in the buffer");
        var buff = data.Skip(ToRemove).Take(size).ToArray();
        ToRemove += size;
        return buff;
    }

    public byte[] ReadAll()
    {
        return ReadBuffer(data.Count - ToRemove);
    }

    public byte[] Peek(int size) => data.Skip(ToRemove).Take(size).ToArray();

    public IRPacket ReadPacket()
    {
        var p = Peek(2);// Dynamic sizing
        if (p.Length < 2) throw new Exception("Not packet size");
        IPacket sizePacket = new Packet();
        sizePacket.Init(p);
        if (sizePacket.Size > Remaining) throw new Exception("Packet in in buffer");
        IRPacket retunValue = new RPacket();
        retunValue.Init(ReadBuffer(sizePacket.Size));
        return retunValue;
    }

    public void AddData(IEnumerable<byte> data)
    {
        this.data.AddRange(data);
    }

    public void AddData(IPacket pkt)
    {
        AddData(pkt.GetData());
        pkt.Final();
    }
}
