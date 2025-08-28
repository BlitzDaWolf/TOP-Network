using System;
using System.Net;
using TOP_Network;
using TOP_Network.Interfaces;
using TOP_Network.Interfaces.Network;
using TOP_Network.Interfaces.Packets;

namespace PacketConnectionTests.Abstractions.Facotries;

public class PacketConnectionFactory : IConectionFactory
{
    public class PacketNetowrkConnection : INetworkConnection
    {
        public Action<IRPacket> OnPacketRecive { get; set; } = _ => { };
        public INetworkBuffer ReciveBuffer { get; set; } = new NetworkBuffer();
        public INetworkBuffer SendBuffer { get; set; } = new NetworkBuffer();

        public INetworkStream Stream { get; }
        public bool IsStep { get; private set; } = false;

        public async Task Next()
        {
            IsStep = true;
            await Task.Delay(450);
        }

        public void Close()
        {
            
        }

        public async Task ReciveLoop()
        {
            while (true)
                await Task.Delay(TimeSpan.FromSeconds(30));
        }

        public async Task SendLoop()
        {
            while (true)
            {
                while (!IsStep) await Task.Delay(1);
                OnPacketRecive(ReciveBuffer.ReadPacket());
            }
        }
    }

    public PacketNetowrkConnection Default { get; set; } = new PacketNetowrkConnection();
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
        return Default;
    }

    public INetworkConnection CreateConnection()
    {
        return Default;
    }

    public void StartListener(IPAddress ip, int port)
    {
        throw new NotImplementedException();
    }
}
