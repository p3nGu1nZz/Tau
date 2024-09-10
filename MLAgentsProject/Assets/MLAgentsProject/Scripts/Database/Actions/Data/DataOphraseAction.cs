using CommandTerminal;
using System;
using System.Diagnostics;

public static class DataOphraseAction
{
    public static async void Execute(CommandArg[] args)
    {
        if (args.Length < 1)
        {
            Log.Error("Invalid command or insufficient arguments. Usage: data ophrase <jsonDataFilename>");
            return;
        }

        string jsonDataFilename = args[0].String;
        Log.Message($"Processing file: {jsonDataFilename}");

        if (Terminal.IssuedError) return;

        Stopwatch stopwatch = Stopwatch.StartNew();
        Log.Message($"Starting to process data from {jsonDataFilename}...");

        var messageList = DataLoader.Load(jsonDataFilename);
        if (messageList == null)
        {
            Log.Error($"Failed to load data from '{jsonDataFilename}'.");
            return;
        }

        var paraphraseTask = new OllamaParaphraseTask();
        try
        {
            await paraphraseTask.Process(messageList, jsonDataFilename);
        }
        catch (Exception ex)
        {
            Log.Error($"Processing failed: {ex.Message}");
            return;
        }

        stopwatch.Stop();
        Log.Message($"Processing completed successfully! (Elapsed time: {stopwatch.Elapsed.TotalSeconds:F2} seconds)");
    }
}
