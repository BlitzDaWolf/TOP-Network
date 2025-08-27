using System;
using System.Net;

namespace TOP_Network.Interfaces;

public interface IConectionFactory
{
    public INetworkConnection CreateConnection();
    public INetworkConnection AcceptConnection(IPAddress ip, int port);
}
