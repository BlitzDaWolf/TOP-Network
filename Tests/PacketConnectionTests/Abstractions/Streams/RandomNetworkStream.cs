using System;
using TOP_Network.Interfaces.Network;

namespace PacketConnectionTests.Abstractions.Streams;

public class RandomNetworkStream : INetworkStream
{
    private readonly byte[] Data;

    private bool IsStep = false;

    public RandomNetworkStream(byte[] data)
    {
        Data = data;
    }
    internal void Step()
    {
        IsStep = true;
    }

    public void Close()
    {
        throw new NotImplementedException();
    }

    public async Task<int> ReadAsync(byte[] buffer)
    {
        while (!IsStep) await Task.Delay(1);
        Array.Copy(Data, buffer, Data.Length);
        IsStep = false;
        return Data.Length;
    }

    public Task WriteAsync(byte[] buffer)
    {
        return Task.CompletedTask;
    }
}
