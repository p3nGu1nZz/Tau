using CommandTerminal;
using System.Diagnostics;
using System.Collections.Generic;

public static class LoadCommand
{
    [RegisterCommand(Help = "Loads training data from one or more JSON files in the Scripts directory", MinArgCount = 1)]
    public static async void CommandLoad(CommandArg[] args)
    {
        List<string> fileNames = new List<string>();
        foreach (var arg in args)
        {
            fileNames.Add(arg.String);
        }

        if (Terminal.IssuedError) return;

        Stopwatch stopwatch = Stopwatch.StartNew();
        Log.Message($"Starting to load training data from {string.Join(", ", fileNames)}...");

        if (fileNames.Count == 1)
        {
            // Single file load
            string fileName = fileNames[0];
            MessageList messageList = DataLoader.Load(fileName);

            if (messageList != null)
            {
                Log.Message("Training data loaded successfully.");
                Log.Message($"Version: {messageList.version}");
                Log.Message($"Model Name: {messageList.model_name}");
                Log.Message($"Organization: {messageList.organization}");

                // Log the size of training and evaluation data
                Log.Message($"Training data size: {messageList.training_data.Count}");
                Log.Message($"Evaluation data size: {messageList.evaluation_data.Count}");

                await DataLoader.LoadData(messageList, fileName);

                stopwatch.Stop();
                Log.Message($"Loading '{fileName}' completed successfully! (Elapsed time: {stopwatch.Elapsed.TotalSeconds} seconds)");
            }
            else
            {
                Log.Error("Failed to load training data.");
            }
        }
        else
        {
            // Multiple files load and concatenate
            string outputFileName = "combined_data.json";
            MessageList combinedMessageList = new MessageList
            {
                version = "0.1.0",
                model_name = "Tau",
                organization = "Huggingface",
                training_data = new List<Message>(),
                evaluation_data = new List<Message>()
            };

            foreach (var fileName in fileNames)
            {
                MessageList messageList = DataLoader.Load(fileName);
                if (messageList != null)
                {
                    combinedMessageList.training_data.AddRange(messageList.training_data);
                    combinedMessageList.evaluation_data.AddRange(messageList.evaluation_data);
                }
            }

            DataLoader.Save(combinedMessageList, outputFileName);
            Log.Message($"Combined data saved to {outputFileName}");

            await DataLoader.LoadData(combinedMessageList, outputFileName);

            stopwatch.Stop();
            Log.Message($"Concatenation and loading completed successfully! (Elapsed time: {stopwatch.Elapsed.TotalSeconds} seconds)");
        }
    }
}
