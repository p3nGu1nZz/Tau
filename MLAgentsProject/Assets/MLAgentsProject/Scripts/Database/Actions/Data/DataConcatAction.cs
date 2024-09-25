using CommandTerminal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

public static class DataConcatAction
{
    public static void Execute(CommandArg[] args)
    {
        try
        {
            bool isFirstFile = true;
            string firstArg = args[0].String;
            string firstArgPath = DataUtilities.GetFilePath(firstArg);
            string saveFileName = Constants.DefaultDataFileName;
            List<string> fileNames = new();
            MessageList combined = new();

            if (Directory.Exists(firstArgPath))
            {
                if (args.Length > 1)
                {
                    throw new ArgumentException("Only one directory can be specified at a time.");
                }

                Log.Message($"Directory detected: {firstArgPath}");
                fileNames = DataUtilities.GetDirectoryContents(firstArgPath, "*.json");
                saveFileName = $"{Path.GetFileName(firstArgPath)}.json";
            }
            else
            {
                string combinedArgs = DataUtilities.CombineArgs(args);
                Log.Message($"Combined arguments: {combinedArgs}");

                fileNames = DataUtilities.ParseFileNames(combinedArgs);
                Log.Message($"Parsed file names: {string.Join(", ", fileNames)}");

                if (fileNames.Count == 0)
                {
                    throw new Exception("No files specified for concatenation.");
                }
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            Log.Message($"Starting to concatenate data from {string.Join(", ", fileNames)}...");

            int totalTrainingRecords = 0;
            int totalEvaluationRecords = 0;

            foreach (var fileName in fileNames)
            {
                string filePath = Directory.Exists(firstArgPath) ? Path.Combine(firstArgPath, fileName) : fileName;
                Log.Message($"Loading data from file: {filePath}");
                MessageList messageList = DataLoader.Load(filePath);

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

                    totalTrainingRecords += messageList.training_data?.Count ?? 0;
                    totalEvaluationRecords += messageList.evaluation_data?.Count ?? 0;

                    Log.Message($"Added data from '{fileName}' into combined data.");
                }
                else
                {
                    throw new Exception($"Failed to load training data from '{fileName}'.");
                }
            }

            Log.Message("Removing duplicates from combined data...");
            DataUtilities.RemoveDuplicates(combined);

            Log.Message("Randomizing combined data...");
            DataUtilities.Shuffle(combined);

            Log.Message("Saving combined data...");
            DataLoader.Save(combined, saveFileName);

            Log.Message($"Total training records added: {totalTrainingRecords}");
            Log.Message($"Total evaluation records added: {totalEvaluationRecords}");
            Log.Message($"Total combined records added: {totalTrainingRecords + totalEvaluationRecords}");
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
