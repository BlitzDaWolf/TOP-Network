using System.Reflection;
using TOP_Network.Packets;

namespace TOP_Network.Converter
{
    public static class ClassToPacket
    {
        public static Packet Convert(object entity)
        {
            Dictionary<string, PropertyInfo> v = new Dictionary<string, PropertyInfo>();
            ClassList.GetList(entity.GetType(), v);

            Packet packet = new Packet();
            packet.Init(new byte[0]);
            using var writer = packet.GetBitWriter();



            return packet;
        }
    }
}
