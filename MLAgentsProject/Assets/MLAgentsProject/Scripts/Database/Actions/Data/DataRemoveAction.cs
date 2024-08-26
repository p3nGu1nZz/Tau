using CommandTerminal;
using System;
using System.IO;

public static class DataRemoveAction
{
    public static void Execute(CommandArg[] args)
    {
        try
        {
            if (args.Length < 1)
            {
                throw new ArgumentException("No file specified to remove.");
            }

            string fileName = args[0].String;
            string filePath = DataUtilities.GetFilePath(fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Log.Message($"File '{fileName}' has been removed successfully.");
            }
            else
            {
                throw new FileNotFoundException($"File '{fileName}' not found.");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while removing the file: {ex.Message}");
            throw;
        }
    }
}
