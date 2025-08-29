using System;

namespace TOP_Network.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ClientAttribute : ConnectionAttribute
{
    public readonly bool Wait;

    public ClientAttribute(bool wait)
    {
        Wait = wait;
    }
}
