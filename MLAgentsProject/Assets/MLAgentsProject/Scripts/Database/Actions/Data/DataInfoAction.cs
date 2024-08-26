using CommandTerminal;
using System;
using System.IO;

public static class DataInfoAction
{
    public static void Execute(CommandArg[] args)
    {
        try
        {
            if (args.Length < 1)
            {
                throw new ArgumentException("No file specified for info.");
            }

            string fileName = args[0].String;
            string filePath = DataUtilities.GetFilePath(fileName);
            MessageList messageList = DataLoader.Load(fileName);

            if (messageList != null)
            {
                FileInfo fileInfo = new FileInfo(filePath);

                Log.Message($"File: {fileName}");
                Log.Message($"Version: {messageList.version}");
                Log.Message($"Model Name: {messageList.model_name}");
                Log.Message($"Organization: {messageList.organization}");
                Log.Message($"Training data count: {messageList.training_data?.Count ?? 0}");
                Log.Message($"Evaluation data count: {messageList.evaluation_data?.Count ?? 0}");
                Log.Message($"Size: {fileInfo.Length} bytes");
                Log.Message($"Created: {fileInfo.CreationTime}");
                Log.Message($"Last Modified: {fileInfo.LastWriteTime}");
            }
            else
            {
                throw new Exception($"Failed to load data from '{fileName}'.");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while executing the info command: {ex.Message}");
            throw;
        }
    }
}
