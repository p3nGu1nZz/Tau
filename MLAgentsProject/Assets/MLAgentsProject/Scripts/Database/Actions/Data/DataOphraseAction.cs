using CommandTerminal;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public static class DataOphraseAction
{
    private static SemaphoreSlim _semaphore = new SemaphoreSlim(5);  // Adjust the number of parallel tasks as needed
    private static CancellationTokenSource _cts = new CancellationTokenSource();

    public static async void Execute(CommandArg[] args)
    {
        string jsonDataFilename = args[0].String;
        Log.Message($"Processing file: {jsonDataFilename}");

        if (Terminal.IssuedError) return;

        Stopwatch stopwatch = Stopwatch.StartNew();
        Log.Message($"Starting to process data from {jsonDataFilename}...");

        // Load the JSON data file
        MessageList messageList = DataLoader.Load(jsonDataFilename);
        if (messageList == null)
        {
            Log.Error($"Failed to load data from '{jsonDataFilename}'.");
            return;
        }

        // Process messages using OllamaParaphraseTask
        var paraphraseTask = new OllamaParaphraseTask(_semaphore, _cts.Token);
        await paraphraseTask.ProcessMessages(messageList, jsonDataFilename);

        stopwatch.Stop();
        Log.Message($"Processing completed successfully! (Elapsed time: {stopwatch.Elapsed.TotalSeconds} seconds)");
    }
}
