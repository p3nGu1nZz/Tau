using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class Optimizer
{
    private static Optimizer _instance;
    public static Optimizer Instance => _instance ??= new Optimizer();

    private static string batchFilePath;

    private Optimizer()
    {
        batchFilePath = Path.Combine(Application.dataPath, Constants.UpDirectoryLevel, Constants.ScriptsDirectoryName, "optimizer.bat");
    }

    public async Task<string> Optimize(string inputFile)
    {
        string command = $"/c \"{batchFilePath} \"{inputFile}\"\"";
        Log.Message($"Executing command: {command}");

        ProcessStartInfo start = new()
        {
            FileName = "cmd.exe",
            Arguments = command,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        Log.Message($"Starting process with arguments: {start.Arguments}");

        try
        {
            using Process process = Process.Start(start);
            using StreamReader reader = process.StandardOutput;
            using StreamReader errorReader = process.StandardError;

            string result = "";
            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                result += line + Environment.NewLine;
            }

            if (result.EndsWith(Environment.NewLine))
            {
                result = result[..^Environment.NewLine.Length];
            }

            string error = await errorReader.ReadToEndAsync();
            if (!string.IsNullOrEmpty(error))
            {
                Log.Error($"Process error: {error}");
            }

            Log.Message($"Process result: {result}");
            return result;
        }
        catch (Exception ex)
        {
            Log.Error($"Unhandled exception occurred: {ex.Message}");
            throw;
        }
    }
}
