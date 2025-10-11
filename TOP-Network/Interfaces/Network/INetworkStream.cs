using System;

namespace TOP_Network.Interfaces.Network;

public interface INetworkStream
{
    public Task<int> ReadAsync(byte[] buffer);
    public Task WriteAsync(byte[] buffer);

    public void Close();
    public uint GetIP();
}
