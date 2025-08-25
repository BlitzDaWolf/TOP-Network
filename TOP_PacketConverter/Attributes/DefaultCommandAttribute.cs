using TOP_Network.Enum;

namespace TOP_PacketConverter.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DefaultCommandAttribute : Attribute
    {
        public readonly Commands Command;

        public DefaultCommandAttribute(Commands command)
        {
            Command = command;
        }
    }
}
