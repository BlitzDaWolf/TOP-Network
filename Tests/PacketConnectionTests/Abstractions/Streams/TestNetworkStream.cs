using System;
using Microsoft.VisualStudio.TestPlatform.Common.DataCollection;
using TOP_Network.Interfaces.Network;

namespace PacketConnectionTests.Abstractions.Streams;

public class TestNetworkStream : INetworkStream
{
    private bool IsStep = false;

    internal void Step()
    {
        IsStep = true;
    }

    public void Close() { }

    public async Task<int> ReadAsync(byte[] buffer)
    {
        while (!IsStep) await Task.Delay(1);
        IsStep = false;
        return 0;
    }

    public async Task WriteAsync(byte[] buffer)
    {
        while (true)
        {
            await Task.Delay(5);
        }
    }
}