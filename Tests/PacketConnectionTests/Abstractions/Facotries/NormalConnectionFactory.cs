using System;
using System.Net;
using TOP_Network;
using TOP_Network.Interfaces;
using TOP_Network.Interfaces.Network;
using TOP_Network.Interfaces.Packets;

namespace PacketConnectionTests.Abstractions.Facotries;

public class NormalConnectionFactory : IConectionFactory
{
    public class NormalNetowrkConnection : INetworkConnection
    {
        public int timing { get; set; } = 10;
        public Action<IRPacket> OnPacketRecive { get; set; } = _ => { };
        public INetworkBuffer ReciveBuffer { get; set; } = new NetworkBuffer();
        public INetworkBuffer SendBuffer { get; set; } = new NetworkBuffer();

        public INetworkStream Stream { get; }

        public void Next()
        {
            ReciveBuffer.AddData(SendBuffer.ReadAll());
        }

        public void Close()
        {

        }

        public Task ReciveLoop() => Task.Delay(TimeSpan.FromSeconds(timing));

        public Task SendLoop() => Task.Delay(TimeSpan.FromSeconds(timing));
        public uint GetIP() => 0;
    }

    public NormalNetowrkConnection Default { get; set; } = new NormalNetowrkConnection();
    public bool IsStep { get; private set; } = false;
    public int timing { get; set; } = 10;

    public async Task Next()
    {
        IsStep = true;
        await Task.Delay(450);
    }

    public async Task<INetworkConnection> AcceptConnection()
    {
        while (!IsStep) await Task.Delay(1);
        IsStep = false;
        return new NormalNetowrkConnection { timing = timing };
    }

    public INetworkConnection CreateConnection(IPAddress ip, int port)
    {
        return new NormalNetowrkConnection { timing = timing };
    }

    public void StartListener(IPAddress ip, int port)
    {

    }
}
