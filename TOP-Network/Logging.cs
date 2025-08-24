using System;
using System.Diagnostics;

namespace TOP_Network;

public static class Logging
{
    public enum LogLevels
    {
        Info = 1,
        Important = 0x33,
        Warning = 0xAA,
        Error = 0xFF
    }

    public static LogLevels LogLevel = 0;

    private static void Write(string Value, LogLevels severity, params object[] args)
    {
        if (severity < LogLevel)
        {
            return;
        }
        var t = new StackTrace(true);
        var f = t.GetFrame(2)!;
        DateTime currentTime = DateTime.Now;
        var fn = Path.GetFileName(f.GetFileName());
        Console.WriteLine($"[{fn}:{f.GetFileLineNumber()}] [{currentTime}]: {string.Format(Value.ToString()!, args)}");
    }

    public static void LogError(object Value)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Write(Value.ToString()!, LogLevels.Error);
    }

    public static void LogImportant(string Value)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Write(Value, LogLevels.Important);
    }

    public static void LogWarning(string Value)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Write(Value, LogLevels.Warning);
    }    public static void LogWarning(string Value, params object[] args)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Write(Value, LogLevels.Warning, args);
    }

    public static void LogInfo(string Value, params object[] args)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Write(Value, LogLevels.Info, args);
    }

    public static void LogInfo(string Value)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Write(Value.Replace("{", "*"), LogLevels.Info);
    }

    public static void LogTodo(object v)
    {
#if DEBUG
        LogWarning("TODO: " + v);
#endif
    }
}
