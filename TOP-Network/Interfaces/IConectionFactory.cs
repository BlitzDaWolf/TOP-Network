using System;
using System.Net;

namespace TOP_Network.Interfaces;

public interface IConectionFactory
{
    public INetworkConnection CreateConnection();
    public void StartListener(IPAddress ip, int port);
    public Task<INetworkConnection> AcceptConnection();
}
