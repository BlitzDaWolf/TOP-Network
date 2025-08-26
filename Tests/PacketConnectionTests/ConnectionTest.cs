using Microsoft.Extensions.Logging;
using TOP_Network;

namespace PacketConnectionTests;

public class ConnectionTest : Connection
{
    public ConnectionTest(int maxClients = 10) : base(LoggerFactory.Create(conf => conf.ClearProviders()).CreateLogger<ConnectionTest>(), maxClients)
    {
    }

    [Fact]
    public async Task StartClient()
    {
        StartAsClient();

        await Task.Delay(5);

        Assert.True(IsConnected());
    }
}
