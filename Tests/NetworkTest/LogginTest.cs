using System;
using TOP_Network;

namespace NetworkTest;

public class LogginTest
{
    [Fact]
    public void WriteInfo()
    {
        Logging.LogInfo("Testlog");
    }

    [Fact]
    public void WriteInfoArgs()
    {
        Logging.LogInfo("Test {0}", 5);
    }

    [Fact]
    public void WriteWarning()
    {
        Logging.LogWarning("Testlog");
    }

    [Fact]
    public void WriteWarningArgs()
    {
        Logging.LogWarning("Test {0}", 5);
    }
}
