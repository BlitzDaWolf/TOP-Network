using System;
using TOP_Network.Interfaces.Packets;
using TOP_Network.Packets;
using TOP_Utils;

namespace TOP_Network;

public class NetworkBuffer
{
    List<byte> data = new List<byte>();

    private int ToRemove = 0;
    private int Remaining => data.Count - ToRemove;
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
        if (Remaining < size) return Array.Empty<byte>();
        var buff = data.Skip(ToRemove).Take(size).ToArray();
        ToRemove += size;
        return buff;
    }

    public byte[] ReadAll()
    {
        return ReadBuffer(data.Count - ToRemove);
    }

    public byte[] Peek(int size) => data.Skip(ToRemove).Take(size).ToArray();

    public RPacket ReadPacket()
    {
        throw new NotImplementedException();
        /*var p = Peek(Packet.StartSize).Reverse().ToArray();
        if (p.Length < Packet.StartSize) return new RPacket(new byte[Packet.StartSize]);
        int sz = Packet.LongSize? (int)BitConverter.ToUInt32(p, 0): (int)BitConverter.ToUInt16(p, 0);
        if (sz < Packet.StartSize)return new RPacket(new byte[Packet.StartSize]);
        if(Remaining < sz)return new RPacket(new byte[Packet.StartSize]);
        return new RPacket(ReadBuffer(sz));*/
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
