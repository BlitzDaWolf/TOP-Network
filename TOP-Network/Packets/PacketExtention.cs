namespace TOP_Network.Packets;

public static class PacketExtention
{
    public static RPacket GetRPacket(this Packet packet)
    {
        RPacket rpkt = new RPacket(packet.Data);
        packet.Final();
        packet = rpkt;
        return rpkt;
    }
}