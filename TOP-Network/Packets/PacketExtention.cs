using TOP_Network.Interfaces.Packets;

namespace TOP_Network.Packets;

public static class PacketExtention
{
    /*public static IRPacket GetRPacket(this IPacket packet, bool reset = true)
    {
        / *RPacket rpkt = new RPacket(packet.Data.ToArray());
        if(reset)
            packet.Final();* /
        return packet.Clone<RPacket>();
    }*/
}