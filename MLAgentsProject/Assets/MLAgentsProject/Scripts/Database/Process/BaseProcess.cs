using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public abstract class BaseProcess : MonoBehaviour
{
    public string BatchFilePath { get; private set; }

    protected virtual void Awake()
    {
        BatchFilePath = Path.Combine(Application.dataPath, "..", "Scripts", GetBatchFileName());
    }

    protected abstract string GetBatchFileName();

    public async Task<string[]> Execute(string inputString)
    {
        string command = $"/c \"{BatchFilePath} \"{inputString}\"\"";
        Log.Message($"Executing command: {command}");

        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = command,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        Log.Message($"Process start info configured with arguments: {startInfo.Arguments}");

        try
        {
            using (var process = Process.Start(startInfo))
            {
                string result = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();

                if (!string.IsNullOrEmpty(error))
                {
                    Log.Error($"Process error: {error}");
                }

                Log.Message($"Process output: {result}");

                try
                {
                    var commandResult = JsonUtility.FromJson<CommandResult>(result);
                    Log.Message($"Parsed {commandResult.responses.Length} responses.");
                    return commandResult.responses;
                }
                catch (Exception jsonEx)
                {
                    Log.Error($"JSON parse error: {jsonEx.Message}");
                    throw;
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Exception occurred during command execution: {ex.Message}");
            throw;
        }
    }
}
