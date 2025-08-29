using System;
using TOP_Network.Enum;

namespace TOP_Network.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class PacketHandleAttribute : Attribute
{
    public readonly Commands CommandType;

    public PacketHandleAttribute(Commands commandType)
    {
        CommandType = commandType;
    }
}
