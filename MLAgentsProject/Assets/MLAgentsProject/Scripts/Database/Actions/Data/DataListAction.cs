using CommandTerminal;
using System;
using System.IO;
using UnityEngine;

public static class DataListAction
{
    public static void Execute(CommandArg[] args)
    {
        try
        {
            string dataDirectory = Path.Combine(Application.dataPath, Constants.UpDirectoryLevel, Constants.DataDirectoryName);
            var jsonFiles = DataUtilities.GetDirectoryContents(dataDirectory, "*.json");

            if (jsonFiles.Count == 0)
            {
                Log.Message("No JSON data files found in the Data directory.");
            }
            else
            {
                Log.Message("JSON data files in the Data directory:");
                Log.Message(new string('-', 100));
                Log.Message($"{"Filename",-30} {"Size (bytes)",-15} {"Created",-25} {"Last Modified",-25}");
                Log.Message(new string('-', 100));
                foreach (var file in jsonFiles)
                {
                    string filePath = Path.Combine(dataDirectory, file);
                    FileInfo fileInfo = new FileInfo(filePath);
                    Log.Message($"{file,-30} {fileInfo.Length,-15} {fileInfo.CreationTime,-25} {fileInfo.LastWriteTime,-25}");
                }
                Log.Message(new string('-', 100));
            }
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while listing the data files: {ex.Message}");
            throw;
        }
    }
}
