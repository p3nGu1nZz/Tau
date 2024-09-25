using CommandTerminal;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public static class DataLoadAction
{
    public static async void Execute(CommandArg[] args)
    {
        string combinedArgs = string.Join(" ", args.Select(arg => arg.String));
        Log.Message($"Combined arguments: {combinedArgs}");

        var matches = Regex.Matches(combinedArgs, @"\""(.*?)\""|\S+");
        List<string> fileNames = matches.Cast<Match>().Select(m => m.Value.Trim('"')).ToList();
        Log.Message($"Parsed file names: {string.Join(", ", fileNames)}");

        if (Terminal.IssuedError) return;

        Stopwatch stopwatch = Stopwatch.StartNew();
        Log.Message($"Starting to load training data from {string.Join(", ", fileNames)}...");

        MessageList combinedMessageList = new MessageList
        {
            version = Constants.Version,
            model_name = Constants.ModelName,
            organization = Constants.Organization,
            training_data = new List<Message>(),
            evaluation_data = new List<Message>()
        };

        foreach (var fileName in fileNames)
        {
            Log.Message($"Loading file: {fileName}");
            MessageList messageList = DataLoader.Load(fileName);

            if (messageList != null)
            {
                Log.Message("Training data loaded successfully.");
                Log.Message($"Version: {messageList.version}");
                Log.Message($"Model Name: {messageList.model_name}");
                Log.Message($"Organization: {messageList.organization}");
                Log.Message($"Training data size: {messageList.training_data.Count}");
                Log.Message($"Evaluation data size: {messageList.evaluation_data.Count}");

                combinedMessageList.training_data.AddRange(messageList.training_data);
                combinedMessageList.evaluation_data.AddRange(messageList.evaluation_data);

                Log.Message($"Appended data from '{fileName}' to combined message list.");
            }
            else
            {
                Log.Error($"Failed to load training data from '{fileName}'.");
                break;
            }
        }

        await DataLoader.LoadData(combinedMessageList);

        stopwatch.Stop();
        Log.Message($"Loading completed successfully! (Elapsed time: {stopwatch.Elapsed.TotalSeconds} seconds)");
    }
}
