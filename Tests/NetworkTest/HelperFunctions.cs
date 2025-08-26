using System;
using System.Security.Cryptography;
using TOP_Network.Interfaces.Packets;
using TOP_Network.Packets;

namespace NetworkTest;

public static class HelperFunctions
{
    public static void HelperSize(IPacket pkt, int expectedSize)
    {
        Assert.Equal(expectedSize, pkt.Size);
        Assert.Equal(expectedSize, pkt.Data.Length);
    }
}
