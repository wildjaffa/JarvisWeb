using System.Diagnostics;
using Microsoft.Extensions.Logging;

public static class BashUtilities
{
    public static async Task RunCommandWithBash(string command, ILogger logger)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "/bin/bash",
            Arguments = command,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);

        await process.WaitForExitAsync();

        var output = process.StandardOutput.ReadToEnd();
        logger.LogDebug(output);
    }
}