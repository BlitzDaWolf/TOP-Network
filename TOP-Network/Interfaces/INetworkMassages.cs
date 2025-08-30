using System;
using System.Reflection;
using Spectre.Console.Cli;
using TOP_Network.Enum;
using TOP_Network.Interfaces.Packets;

namespace TOP_Network.Interfaces;

public interface INetworkCommand
{
    public Dictionary<Commands, MethodInfo> ComamndMethods { get; }

    protected void InitCommands();
    public void DisplayComamnds();
    public void OnPreHandel(IRPacket packet, int connection, IMethodBag Bag);
    public bool TyHandlePacket(IRPacket packet, int connection, out IPacket ReplyPacket);
}
