using TOP_Network.Interfaces.Packets;

namespace TOP_Network.Interfaces;

public interface INetworkBuffer
{
    public List<byte> Data { get; }
    public bool EOF { get; }
    public int Remaining { get; }

    public void SafeStep();
    public byte[] ReadBuffer(int size);
    public byte[] ReadAll();
    public byte[] Peek(int size);

    public IRPacket ReadPacket();
    public void AddData(IEnumerable<byte> data);
    public void AddData(IPacket pkt);
}
