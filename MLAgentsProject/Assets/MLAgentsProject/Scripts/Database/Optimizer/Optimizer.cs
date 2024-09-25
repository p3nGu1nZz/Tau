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
        Console.WriteLine($"Command: {command}");
        Log.Message($"Executing command: {command}");

        ProcessStartInfo start = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = command,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        Console.WriteLine($"Starting process with arguments: {start.Arguments}");
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
                result = result.Substring(0, result.Length - Environment.NewLine.Length);
            }

            string error = await errorReader.ReadToEndAsync();
            if (!string.IsNullOrEmpty(error))
            {
                Console.Error.WriteLine($"Process error: {error}");
                Log.Error($"Process error: {error}");
            }

            return result;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Exception occurred: {ex.Message}");
            Log.Error($"Exception occurred: {ex.Message}");
            return null;
        }
    }
}
