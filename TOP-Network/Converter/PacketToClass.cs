using System.Reflection;
using TOP_Network.Packets;

namespace TOP_Network.Converter
{
    public static class PacketToClass
    {
        public static void Convert<T>(this Packet packet)
        {
            Dictionary<string, PropertyInfo> v = new Dictionary<string, PropertyInfo>();
            ClassList.GetList(typeof(T).GetType(), v);
        }
    }
}
