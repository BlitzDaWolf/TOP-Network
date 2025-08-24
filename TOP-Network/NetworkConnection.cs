using System;
using System.Net.Sockets;

namespace TOP_Network;

public struct NetworkConnection
{
    public TcpClient Client { get; set; }
    public NetworkStream Stream { get; set; }
    public NetworkBuffer SendBuffer { get; set; }
    public NetworkBuffer ReciveBuffer { get; set; }
    public bool SendTrace { get; set; }

    public void Close()
    {
        Stream.Close();
        Client.Close();
    }
}