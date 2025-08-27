using System;
using TOP_Network;
using TOP_Network.Interfaces;
using TOP_Network.Interfaces.Network;

namespace PacketConnectionTests.Abstractions;

public class TestConenctionFactory<TBuffer> : IConectionFactory where TBuffer : INetworkBuffer, new()
{
    public INetworkStream UseStream { get; set; }

    public TestConenctionFactory(INetworkStream useStream)
    {
        UseStream = useStream;
    }

    public INetworkConnection CreateConnection()
    {
        throw new NotImplementedException();
    }
}


