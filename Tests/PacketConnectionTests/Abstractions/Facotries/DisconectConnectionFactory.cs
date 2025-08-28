using System;
using System.Net;
using TOP_Network;
using TOP_Network.Interfaces;
using TOP_Network.Interfaces.Network;
using TOP_Network.Interfaces.Packets;

namespace PacketConnectionTests.Abstractions.Facotries;

public class DisconectConnectionFactory : IConectionFactory
{
    public class DisconectNetowrkConnection : INetworkConnection
    {
        public Action<IRPacket> OnPacketRecive { get; set; } = _ => { };
        public INetworkBuffer ReciveBuffer { get; set; } = new NetworkBuffer();
        public INetworkBuffer SendBuffer { get; set; } = new NetworkBuffer();

        public INetworkStream Stream { get; }

        private bool connected = true;

        public void Close()
        {
            connected = false;
        }

        public async Task ReciveLoop()
        {
            while (connected) await Task.Delay(5);
        }

        public async Task SendLoop()
        {
            while (connected) await Task.Delay(5);
        }
    }

    public DisconectNetowrkConnection Default { get; set; } = new DisconectNetowrkConnection();
    public bool IsStep { get; private set; } = false;

    public async Task Next()
    {
        IsStep = true;
        await Task.Delay(450);
    }

    public async Task<INetworkConnection> AcceptConnection()
    {
        while (!IsStep) await Task.Delay(1);
        IsStep = false;
        return new DisconectNetowrkConnection();
    }

    public INetworkConnection CreateConnection(IPAddress ip, int port)
    {
        return new DisconectNetowrkConnection();
    }

    public void StartListener(IPAddress ip, int port)
    {
        
    }
}
