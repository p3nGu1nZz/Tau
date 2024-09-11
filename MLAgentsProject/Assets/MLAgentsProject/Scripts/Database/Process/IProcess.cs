using System.Diagnostics;
using System.Threading.Tasks;

public interface IProcess
{
    string BatchFilePath { get; }
    Task<string[]> Execute(string inputString);
    string BuildCommand(string inputString);
    ProcessStartInfo CreateProcessStartInfo(string command);
    Task<string> ReadProcessOutput(Process process);
    Task<string> ReadProcessError(Process process);
    void HandleProcessError(string error);
    string[] ParseCommandResult(string result);
    void LogExecutionTime(Stopwatch stopwatch);
}
