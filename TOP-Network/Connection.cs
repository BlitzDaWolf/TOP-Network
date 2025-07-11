using System.Net.Sockets;
using TOP_Network.Packets;

namespace TOP_Network;

public class Connection
{
    NetworkStream? stream;

    private Dictionary<uint, Packet?> called = new Dictionary<uint, Packet?>();

    public Connection()
    {

    }

    public async Task connect(TcpClient groupServer)
    {
        stream = groupServer.GetStream();

        try
        {
            var _ = OnConnected();
            bool IsTransaction = false;

            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                if (IsTransaction)
                {

                }
                else
                {
                    var t = new List<byte>(buffer.Take(bytesRead));
                    while (t.Count != 0)
                    {
                        Packet pkt = new Packet(t.ToArray());
                        if (pkt.Size == 0)
                        {
                            break;
                        }
                        if (pkt.Size <= 6)
                        {
                            t.RemoveRange(0, Math.Max(1, pkt.Size));
                            if(pkt.Size == 2)
                            {
                                await Send(pkt);
                            }
                            continue;
                        }
                        if (pkt.Size > buffer.Length)
                        {
                            IsTransaction = true;
                        }
                        else
                        {
                            pkt = new Packet(t.Take(pkt.Size).ToArray());
                            try
                            {
                                await handelPacket(pkt);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        }
                        t.RemoveRange(0, pkt.Size);
                    }
                }
                IsTransaction = false;
            }
        }
        catch
        {

        }
    }

    private async Task handelPacket(Packet pkt)
    {
        Console.WriteLine("A new packet arived: " + pkt.gnack);
        if (called.ContainsKey(pkt.gnack))
        {
            called[pkt.gnack] = pkt;
            return;
        }
        await HandelPacket(pkt);
    }

    public virtual Task OnConnected() => Task.CompletedTask;
    public virtual Task HandelPacket(Packet pkt) => Task.CompletedTask;

    public async Task Send(Packet pkt)
    {
        await stream!.WriteAsync(pkt.Data, 0, pkt.Size);
        await stream!.FlushAsync();
    }

    public uint packet { get; private set; } = 0;

    public async Task<Packet?> SyncCall(Packet pkt, int timeout = 10_000)
    {
        // pkt.AddRandomGnack();
        pkt.WriteNewGnack(++packet);
        uint test = pkt.gnack + 2147483648;

        await Send(pkt);
        called.Add(test, null);
        var delay = Task.Delay(timeout);
        while (called[test] == null && !delay.IsCompleted)
        {
            await Task.Delay(100);
        }
        var result = called[test];
        called.Remove(test);
        return result;
    } 
}
