using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public abstract class BaseProcess<T> : IProcess where T : BaseProcess<T>
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

            using (var process = StartProcess(startInfo))
            {
                string result = await ReadProcessOutput(process);
                string error = await ReadProcessError(process);

                HandleProcessError(error);

                return ParseCommandResult(result);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Command execution failed: {ex.Message}", ex);
        }
        finally
        {
            LogExecutionTime(stopwatch);
        }
    }

    public string BuildCommand(string inputString) => ProcessUtilities.BuildCommand(BatchFilePath, inputString);

    public ProcessStartInfo CreateProcessStartInfo(string command) => ProcessUtilities.CreateProcessStartInfo(command);

    public Process StartProcess(ProcessStartInfo startInfo) => Process.Start(startInfo);

    public async Task<string> ReadProcessOutput(Process process) => await process.StandardOutput.ReadToEndAsync();

    public async Task<string> ReadProcessError(Process process) => await process.StandardError.ReadToEndAsync();

    public void HandleProcessError(string error)
    {
        if (!string.IsNullOrEmpty(error))
            throw new Exception($"Process exception: {error}");
    }

    public string[] ParseCommandResult(string result) => ProcessUtilities.ParseCommandResult(result);

    public void LogExecutionTime(Stopwatch stopwatch)
    {
        stopwatch.Stop();
        Log.Message($"Command execution completed in {stopwatch.ElapsedMilliseconds / 1000.0:F2} s.");
    }
}
