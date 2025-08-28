using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using TOP_Network.Interfaces;
using TOP_Network.Interfaces.Network;
using TOP_Network.Interfaces.Packets;

namespace TOP_Network;

[ExcludeFromCodeCoverage]
public class ConectionFactory : IConectionFactory
{
    [ExcludeFromCodeCoverage]
    public class NetworkConnection : INetworkConnection
    {
        public Action<IRPacket> OnPacketRecive { get; set; } = _ => { };
        public INetworkBuffer ReciveBuffer { get; set; } = new NetworkBuffer();
        public INetworkBuffer SendBuffer { get; set; } = new NetworkBuffer();

        public INetworkStream Stream { get; private set; }

        public NetworkConnection(INetworkStream stream)
        {
            Stream = stream;
        }

        public void Close()
        {
            Stream.Close();
        }

        public async Task ReciveLoop()
        {
            byte[] buffer = new byte[32_768];
            int bufferRead = 0;
            while ((bufferRead = await Stream.ReadAsync(buffer)) > 0)
            {
                ReciveBuffer.AddData(buffer.Take(bufferRead));
            }
        }

        public async Task SendLoop()
        {
            byte cnt = 0;
            while (true)
            {
                int hasData = (ReciveBuffer.EOF ? 0 : 1) + (SendBuffer.EOF ? 0 : 2);
                if (hasData == 0)
                {
                    await Task.Delay(10);
                }
                if (hasData >= 2)
                {
                    await Stream.WriteAsync(SendBuffer.ReadAll());
                    SendBuffer.SafeStep();
                }
                if (hasData % 2 == 1)
                {
                    IRPacket currentpacket = ReciveBuffer.ReadPacket();
                    /*if (currentpacket.Size == currentpacket.StartSize)
                    {
                        SendBuffer.AddData(currentpacket);
                    }
                    else*/
                    {
                        OnPacketRecive(currentpacket);
                        cnt++;
                        cnt %= 10;
                        if (cnt == 0) ReciveBuffer.SafeStep();
                    }
                }
            }
        }
    }

    [ExcludeFromCodeCoverage]
    public class NetworkStream : INetworkStream
    {
        public readonly TcpClient Client;
        public readonly System.Net.Sockets.NetworkStream Stream;

        public NetworkStream(TcpClient client)
        {
            Client = client;
            Stream = client.GetStream();
        }

        public void Close()
        {
            Stream.Close();
            Client.Close();
        }

        public async Task<int> ReadAsync(byte[] buffer) => await Stream.ReadAsync(buffer, 0, buffer.Count());

        public async Task WriteAsync(byte[] buffer)
        {
            Stream.Write(buffer);
            await Stream.FlushAsync();
        }
    }

    private TcpListener? listener;

    public async Task<INetworkConnection> AcceptConnection()
    {
        try
        {
            TcpClient tcClient = await listener!.AcceptTcpClientAsync();
            return new NetworkConnection(new NetworkStream(tcClient));
        }
        catch
        {
            throw;
        }
    }

    public INetworkConnection CreateConnection(IPAddress ip, int port)
    {
        TcpClient client = new TcpClient(ip.ToString(), port);
        return new NetworkConnection(new NetworkStream(client));
    }

    public void StartListener(IPAddress ip, int port)
    {
        listener = new TcpListener(ip, port);
        listener.Start();
    }
}
