using System;
using TOP_Network.Interfaces.Network;
using TOP_Network.Interfaces.Packets;

namespace TOP_Network.Interfaces;

public interface INetworkConnection
{
    public Action<IRPacket> OnPacketRecive { get; set; }

    public INetworkBuffer ReciveBuffer { get; set; }
    public INetworkBuffer SendBuffer{ get; set; }
    public INetworkStream Stream{ get; }

    public void Close();

    public Task ReciveLoop();
    public Task SendLoop();
}
