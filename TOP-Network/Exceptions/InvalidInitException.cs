using System;

namespace TOP_Network.Exceptions;

public abstract class InvalidInitException : Exception
{
    public InvalidInitException(string reason)
        : base(reason) { }
}

public class InvalidIPInitException : InvalidInitException
{
    public InvalidIPInitException(string reason) : base(reason) { }
}
public class InvalidPortInitException : InvalidInitException
{
    public InvalidPortInitException(string reason) : base(reason) { }
}
