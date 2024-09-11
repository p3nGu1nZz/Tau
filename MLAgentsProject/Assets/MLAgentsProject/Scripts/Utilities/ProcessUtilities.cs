using System.Diagnostics;
using System.IO;
using UnityEngine;

public static class ProcessUtilities
{
    public static string BuildCommand(string batchFilePath, string inputString) =>
        $"/c \"{batchFilePath} \"{inputString}\"\"";

    public static ProcessStartInfo CreateProcessStartInfo(string command) =>
        new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = command,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

    public static string[] ParseCommandResult(string result)
    {
        var commandResult = JsonUtility.FromJson<CommandResult>(result);
        return commandResult.responses;
    }
}
