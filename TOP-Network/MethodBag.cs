using System;
using System.Reflection;
using TOP_Network.Interfaces;

namespace TOP_Network;

public class MethodBag : IMethodBag
{
    public Dictionary<string, IParameterBag> Values { get; private set; } = new Dictionary<string, IParameterBag>();

    public MethodBag(ParameterInfo[] parameterInfos)
    {
        for (int i = 0; i < parameterInfos.Length; i++)
        {
            Values.Add(parameterInfos[i].Name!.ToLower(), new ParameterBag
            {
                Name = parameterInfos[i].Name!,
                ParameterType = parameterInfos[i].ParameterType,
                Value = parameterInfos[i].DefaultValue
            });
        }
    }

    public object? GetValue(string value)
    {
        if (Values.ContainsKey(value.ToLower())) return Values[value.ToLower()].Value;
        return null;
    }

    public void SetValue(string key, object value)
    {
        if (Values.ContainsKey(key.ToLower())) Values[key.ToLower()].SetValue(value);
    }
}

public class ParameterBag : IParameterBag
{
    public string Name { get; init; }
    public Type ParameterType { get; init; }

    public object? Value{ get; set; }

    public void SetValue(object value)
    {
        if (ParameterType == value.GetType() || ParameterType.IsAssignableFrom(value.GetType())) Value = value;
    }
} 