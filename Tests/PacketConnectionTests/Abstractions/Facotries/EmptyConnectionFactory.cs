using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using TOP_Network.Interfaces;
using TOP_Network.Interfaces.Network;
using TOP_Network.Interfaces.Packets;

namespace PacketConnectionTests.Abstractions.Facotries;

[ExcludeFromCodeCoverage]
public class EmptyConnectionFactory : IConectionFactory
{
    [ExcludeFromCodeCoverage]
    public class EmptyNetowrkConnection : INetworkConnection
    {
        public Action<IRPacket> OnPacketRecive { get; set; }
        public INetworkBuffer ReciveBuffer { get; set; }
        public INetworkBuffer SendBuffer { get; set; }

        public INetworkStream Stream { get; }

        public void Close() { }

        public Task ReciveLoop() => Task.CompletedTask;

        public Task SendLoop() => Task.CompletedTask;
    }

    public async Task<INetworkConnection> AcceptConnection()
    {
        await Task.Delay(TimeSpan.FromSeconds(30));
        return new EmptyNetowrkConnection();        
    } 

    public INetworkConnection CreateConnection() => new EmptyNetowrkConnection();

    public void StartListener(IPAddress ip, int port)
    { }
}
