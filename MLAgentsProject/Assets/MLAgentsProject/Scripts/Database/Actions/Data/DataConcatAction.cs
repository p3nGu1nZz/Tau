using CommandTerminal;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public static class DataConcatAction
{
    public static void Execute(CommandArg[] args)
    {
        try
        {
            string combinedArgs = DataUtilities.CombineArgs(args);
            Log.Message($"Combined arguments: {combinedArgs}");

            List<string> fileNames = DataUtilities.ParseFileNames(combinedArgs);
            Log.Message($"Parsed file names: {string.Join(", ", fileNames)}");

            if (fileNames.Count == 0)
            {
                throw new Exception("No files specified for concatenation.");
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            Log.Message($"Starting to concatenate data from {string.Join(", ", fileNames)}...");

            MessageList combined = new MessageList
            {
                training_data = new List<Message>(),
                evaluation_data = new List<Message>()
            };
            bool isFirstFile = true;
            string saveFileName = "combined_data.json";

            foreach (var fileName in fileNames)
            {
                Log.Message($"Loading data from file: {fileName}");
                MessageList messageList = DataLoader.Load(fileName);

                if (messageList != null)
                {
                    Log.Message("Data loaded successfully.");
                    Log.Message($"Version: {messageList.version}");
                    Log.Message($"Model Name: {messageList.model_name}");
                    Log.Message($"Organization: {messageList.organization}");

                    if (isFirstFile)
                    {
                        combined.version = messageList.version;
                        combined.model_name = messageList.model_name;
                        combined.organization = messageList.organization;
                        isFirstFile = false;
                    }

                    Log.Message($"Training data size: {messageList.training_data?.Count}");
                    Log.Message($"Evaluation data size: {messageList.evaluation_data?.Count}");

                    if (messageList.training_data == null)
                    {
                        Log.Error("Training data is null.");
                    }
                    if (messageList.evaluation_data == null)
                    {
                        Log.Error("Evaluation data is null.");
                    }

                    combined.training_data.AddRange(messageList.training_data ?? new List<Message>());
                    combined.evaluation_data.AddRange(messageList.evaluation_data ?? new List<Message>());

                    Log.Message($"Added data from '{fileName}' into combined data.");
                }
                else
                {
                    throw new Exception($"Failed to load training data from '{fileName}'.");
                }
            }

            Log.Message("Saving combined data...");
            DataLoader.Save(combined, saveFileName);
            Log.Message($"Concatenation completed successfully! Combined data saved to '{saveFileName}' (Elapsed time: {stopwatch.Elapsed.TotalSeconds} seconds)");

            stopwatch.Stop();
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred during concatenation: {ex.Message}");
            throw;
        }
    }
}