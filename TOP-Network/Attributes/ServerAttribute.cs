using System;

namespace TOP_Network.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class ServerAttribute : ConnectionAttribute;
