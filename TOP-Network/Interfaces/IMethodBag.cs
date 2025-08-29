using System;
using System.Reflection;

namespace TOP_Network.Interfaces;

public interface IMethodBag
{
    public Dictionary<string, IParameterBag> Values { get; }

    public object? GetValue(string value);
    public void SetValue(string key, object value);
}

public interface IParameterBag
{
    public string Name { get; init; }
    public Type ParameterType { get; init; }
    public object? Value { get; }

    public void SetValue(object value);
}
