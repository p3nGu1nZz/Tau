using CommandTerminal;

public static class DataPruneAction
{
    public static void Execute(CommandArg[] args)
    {
        if (args.Length < 1)
        {
            Log.Error("Please provide the filename to clean.");
            return;
        }

        if (!Database.Instance.IsLoaded())
        {
            Log.Error("No database loaded. Please load a database before cleaning data.");
            return;
        }

        string fileName = args[0].String;
        Log.Message($"Cleaning data file: {fileName}");

        MessageList messageList = DataLoader.Load(fileName);
        if (messageList == null)
        {
            Log.Error($"Failed to load data from '{fileName}'.");
            return;
        }

        Log.Message($"Loaded data file: {fileName}");
        Log.Message($"Initial training data count: {messageList.training_data.Count}");
        Log.Message($"Initial evaluation data count: {messageList.evaluation_data.Count}");

        DataUtilities.CleanPunctuationSpaces(messageList);
        //messageList = DataLoader.Prune(messageList);

        if (messageList != null)
        {
            Log.Message($"Post-pruning training data count: {messageList.training_data.Count}");
            Log.Message($"Post-pruning evaluation data count: {messageList.evaluation_data.Count}");

            // Save the cleaned data
            DataLoader.Save(messageList, fileName);
            Log.Message($"Data file '{fileName}' cleaned and saved successfully.");
        }
        else
        {
            Log.Error($"Failed to prune data from '{fileName}'.");
        }
    }
}
