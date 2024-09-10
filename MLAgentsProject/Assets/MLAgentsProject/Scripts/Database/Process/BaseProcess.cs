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
        var stopwatch = Stopwatch.StartNew();

        try
        {
            string command = $"/c \"{BatchFilePath} \"{inputString}\"\"";
            Log.Message($"Starting command: '{command}'");

            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = command,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (var process = Process.Start(startInfo))
            {
                string result = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();

                if (!string.IsNullOrEmpty(error))
                {
                    throw new Exception($"Process exception: {error}");
                }

                var commandResult = JsonUtility.FromJson<CommandResult>(result);
                return commandResult.responses;
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
}
