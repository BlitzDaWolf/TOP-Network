namespace TOP_Network.Packets;

public static class PacketExtention
{
    public static RPacket GetRPacket(this V1Packet packet, bool reset = true)
    {
        RPacket rpkt = new RPacket(packet.Data.ToArray());
        if(reset)
            packet.Final();
        return rpkt;
    }
}