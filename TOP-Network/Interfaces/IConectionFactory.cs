using System;
using System.Net;

namespace TOP_Network.Interfaces;

public interface IConectionFactory
{
    public INetworkConnection CreateConnection(IPAddress ip, int port);
    public void StartListener(IPAddress ip, int port);
    public Task<INetworkConnection> AcceptConnection();
}
