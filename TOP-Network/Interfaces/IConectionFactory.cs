using System;

namespace TOP_Network.Interfaces;

public interface IConectionFactory
{
    public INetworkConnection CreateConnection();
}
