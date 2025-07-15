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

    private static void Write(object Value, LogLevels severity)
    {
        if (severity < LogLevel)
        {
            return;
        }
        var t = new StackTrace(true);
        var f = t.GetFrame(2)!;

        DateTime currentTime = DateTime.Now;
        var fn = Path.GetFileName(f.GetFileName());
        Console.WriteLine($"[{fn}:{f.GetFileLineNumber()}] [{currentTime}]: {Value}");
    }

    public static void LogError(object Value)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Write(Value, LogLevels.Error);
    }

    public static void LogImportant(object Value)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Write(Value, LogLevels.Important);
    }

    public static void LogWarning(object Value)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Write(Value, LogLevels.Warning);
    }

    public static void LogInfo(object Value)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Write(Value, LogLevels.Info);
    }

    public static void LogTodo(object v)
    {
#if DEBUG
        LogWarning("TODO: " + v);
#endif
    }
}
