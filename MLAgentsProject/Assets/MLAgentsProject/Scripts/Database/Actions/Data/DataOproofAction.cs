using CommandTerminal;
using System;
using System.Diagnostics;

public static class DataOproofAction
{
    public static async void Execute(CommandArg[] args)
    {
        if (args.Length < 1)
        {
            Log.Error("Invalid command or insufficient arguments. Usage: data oproof <jsonDataFilename>");
            return;
        }

        string filename = args[0].String;
        Log.Message($"Processing file: {filename}");

        if (Terminal.IssuedError) return;

        Stopwatch stopwatch = Stopwatch.StartNew();
        Log.Message($"Starting to process data from {filename}...");

        var messageList = DataLoader.Load(filename);
        if (messageList == null)
        {
            Log.Error($"Failed to load data from '{filename}'.");
            return;
        }

        var task = new OllamaProofTask();
        try
        {
            await task.Process(messageList, filename);
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
