using System;
using System.Reflection;
using Microsoft.Extensions.Logging;
using TOP_Network.Attributes;
using TOP_Network.Enum;
using TOP_Network.Interfaces;
using TOP_Network.Interfaces.Packets;

namespace TOP_Network;

public static class NetworkCommand<T> where T : IConnection
{
    public delegate void PreHandelDelegate(IRPacket packet, int connection, IMethodBag Bag);

    private static Dictionary<Commands, MethodInfo> ComamndMethods = new Dictionary<Commands, MethodInfo>();
    public static void InitCommands()
    {
        ComamndMethods = new Dictionary<Commands, MethodInfo>();
        MethodInfo[] methods = typeof(T).GetMethods().Where(x => x.GetCustomAttribute<PacketHandleAttribute>() != null).ToArray();
        for (int i = 0; i < methods.Length; i++)
        {
            ComamndMethods.Add(methods[i].GetCustomAttribute<PacketHandleAttribute>()!.CommandType, methods[i]);
        }
    }

    public static void DisplayMethods(ILogger _logger)
    {
        _logger.LogInformation("Commands in: {0}", typeof(T));
        // for (int i = 0; i < ComamndMethods.Count; i++)
        foreach (var command in ComamndMethods.Keys)
        {
            var name = command.ToString();
            var code = (short)command;

            _logger.LogInformation("Command: {0} | {1}", code, name);
        }
    }

    public static bool TrHandlePacket(T caller, IRPacket packet, int connection, out IPacket? ReplyPacket, PreHandelDelegate OnPreHandel)
    {
        ReplyPacket = null;
        if (ComamndMethods.ContainsKey(packet.Command))
        {
            var methods = ComamndMethods[packet.Command];
            ParameterInfo[] parameters = methods.GetParameters();

            IMethodBag bag = new MethodBag(parameters);
            bag.SetValue("packet", packet);

            OnPreHandel(packet, connection, bag);

            var values = parameters.Select(x => bag.GetValue(x.Name!)).ToArray();

            IPacket? replyPacket = null;

            if (methods.ReturnType == typeof(Task<IPacket>))
            {
                var tmp = (Task<IPacket>)methods.Invoke(caller, values)!;
                tmp.Wait();
                replyPacket = tmp.Result;
            }
            else if (methods.ReturnType == typeof(IPacket))
            {
                replyPacket = (IPacket)methods.Invoke(caller, values)!;
            }
            else
            {
                methods.Invoke(caller, values);
            }
            return true;
        }
        return false;
    }
}