using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class ProcessUtilities
{
    public static string BuildCommand(string batchFilePath, string[] args)
    {
        string joinedArgs = string.Join("\" \"", args);
        return $"/c \"{batchFilePath} \"{joinedArgs}\"\"";
    }

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

    public static List<TResult> ParseResult<TResult>(string result)
    {
        var commandResult = JsonUtility.FromJson<Result<TResult>>(result);
        return commandResult.responses;
    }
}
