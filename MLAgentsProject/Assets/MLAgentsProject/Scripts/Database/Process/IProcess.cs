using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

public interface IProcess<TResult>
{
    string BatchFilePath { get; }
    Task<List<TResult>> Execute(string[] args);
    string BuildCommand(string[] args);
    ProcessStartInfo CreateProcessStartInfo(string command);
    Task<string> ReadProcessOutput(Process process);
    Task<string> ReadProcessError(Process process);
    void HandleProcessError(string error);
    List<TResult> ParseCommandResult(string result);
    void LogExecutionTime(Stopwatch stopwatch);
}
