using System;
using TOP_Network.Packets;

namespace NetworkTest;

public static class HelperFunctions
{
    public static void HelperSize(Packet pkt, int expectedSize)
    {
        Assert.Equal(expectedSize, pkt.Size);
        Assert.Equal(expectedSize, pkt.Data.Length);
    }
}
