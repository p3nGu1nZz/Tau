using CommandTerminal;
using UnityEngine;
using System.IO;

public static class CleanCommand
{
    [RegisterCommand(Help = "Cleans up generated vocab and embedding files", MinArgCount = 0)]
    public static void CommandClean(CommandArg[] args)
    {
        string dataDirectory = Path.Combine(Application.dataPath, "..", "Data");
        string logsDirectory = Path.Combine(Application.dataPath, "..", "Logs");

        string[] filesToDelete = {
            Path.Combine(dataDirectory, "vocabulary.json"),
            Path.Combine(dataDirectory, "database.bin"),
            Path.Combine(dataDirectory, "database.txt"),
            Path.Combine(logsDirectory, "log.txt")
        };

        foreach (var file in filesToDelete)
        {
            try
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                    Debug.Log($"Deleted: {file}");
                }
                else
                {
                    Debug.Log($"File not found: {file}");
                }
            }
            catch (IOException ex)
            {
                Debug.LogError($"Error deleting file {file}: {ex.Message}");
            }
        }

        Debug.Log("Cleanup completed.");
    }
}
