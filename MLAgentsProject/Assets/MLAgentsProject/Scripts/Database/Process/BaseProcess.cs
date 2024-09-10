using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public abstract class BaseProcess
{
    public string BatchFilePath { get; private set; }

    protected BaseProcess()
    {
        BatchFilePath = Path.Combine(Application.dataPath, "..", "Scripts", GetBatchFileName());
    }

    protected abstract string GetBatchFileName();

    public async Task<string[]> Execute(string inputString)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            string command = BuildCommand(inputString);
            Log.Message($"Starting command: '{command}'");

            var startInfo = CreateProcessStartInfo(command);

            using (var process = Process.Start(startInfo))
            {
                string result = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();

                if (!string.IsNullOrEmpty(error))
                {
                    throw new Exception($"Process exception: {error}");
                }

                return ParseCommandResult(result);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Command execution failed: {ex.Message}", ex);
        }
        finally
        {
            stopwatch.Stop();
            Log.Message($"Command execution completed in {stopwatch.ElapsedMilliseconds / 1000.0:F2} s.");
        }
    }

    private string BuildCommand(string inputString)
    {
        return $"/c \"{BatchFilePath} \"{inputString}\"\"";
    }

    private ProcessStartInfo CreateProcessStartInfo(string command)
    {
        return new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = command,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
    }

    private string[] ParseCommandResult(string result)
    {
        var commandResult = JsonUtility.FromJson<CommandResult>(result);
        return commandResult.responses;
    }
}
