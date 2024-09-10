using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class OllamaParaphraseTask : BaseTask<OllamaParaphraseTask>
{
    private static readonly int MaxConcurrency = 8;
    private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(MaxConcurrency);
    private static readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

    private readonly ConcurrentDictionary<string, int> _counters = new ConcurrentDictionary<string, int>();

    public OllamaParaphraseTask()
    {
        Application.quitting += OnApplicationQuit;
    }

    private void OnApplicationQuit()
    {
        CancellationTokenSource.Cancel();
    }

    public override async Task Process(MessageList messageList, string jsonDataFilename)
    {
        _counters["totalProcessedMessages"] = 0;
        _counters["totalGeneratedPhrases"] = 0;

        var newMessagesList = new List<Message>();
        int totalMessages = messageList.training_data.Count;
        int timeoutLength = 30;
        int retryAmount = 5;

        Log.Message($"Starting to process {totalMessages} user contents from {jsonDataFilename}...");

        var stopwatch = Stopwatch.StartNew();

        var tasks = new List<Task>();

        for (int i = 0; i < totalMessages; i++)
        {
            var message = messageList.training_data[i];
            var userContent = GetUserContent(message);

            tasks.Add(ProcessUserContent(userContent, message, newMessagesList, timeoutLength, retryAmount, i, totalMessages));
        }

        await HandleTasksCompletion(tasks);

        stopwatch.Stop();
        LogProcessingCompletion(stopwatch, _counters["totalGeneratedPhrases"], messageList, newMessagesList, _counters["totalProcessedMessages"], jsonDataFilename);
    }

    private async Task ProcessUserContent(string userContent, Message message, List<Message> newMessagesList, int timeoutLength, int retryAmount, int index, int totalMessages)
    {
        await Semaphore.WaitAsync(CancellationTokenSource.Token);
        try
        {
            Log.Message($"Processing user content: {userContent} ({index + 1} of {totalMessages})");

            var responses = await Execute(userContent, TimeSpan.FromSeconds(timeoutLength), retryAmount);
            if (responses.Length == 0 || responses.Any(response => response.StartsWith("Error")))
            {
                throw new Exception($"Failed to generate valid responses for user content: {userContent}");
            }

            var newMessages = CreateNewMessages(message, responses);
            lock (newMessagesList)
            {
                newMessagesList.AddRange(newMessages);
            }
            _counters.AddOrUpdate("totalProcessedMessages", newMessages.Count, (key, oldValue) => oldValue + newMessages.Count);
            _counters.AddOrUpdate("totalGeneratedPhrases", responses.Length, (key, oldValue) => oldValue + responses.Length);

            Log.Message($"Generated {newMessages.Count} responses for user content: {userContent}");
            Log.Message($"Completed {index + 1} of {totalMessages} tasks.");
            Log.Message($"Total phrases generated so far: {_counters["totalGeneratedPhrases"]}");
        }
        catch (Exception)
        {
            CancellationTokenSource.Cancel();
            throw;
        }
        finally
        {
            Semaphore.Release();
        }
    }

    private async Task HandleTasksCompletion(List<Task> tasks)
    {
        try
        {
            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            Log.Error($"Processing failed: {ex.Message}");
            CancellationTokenSource.Cancel();
            throw;
        }
    }

    private void LogProcessingCompletion(Stopwatch stopwatch, int totalGeneratedPhrases, MessageList messageList, List<Message> newMessagesList, int totalProcessedMessages, string jsonDataFilename)
    {
        double elapsedMinutes = stopwatch.Elapsed.TotalMinutes;
        double paraphrasesPerMinute = totalGeneratedPhrases / elapsedMinutes;

        Log.Message($"Processing completed in {stopwatch.ElapsedMilliseconds} ms.");
        Log.Message($"Total paraphrases generated per minute: {paraphrasesPerMinute:F2}");

        lock (messageList.training_data)
        {
            messageList.training_data.AddRange(newMessagesList);
        }

        Log.Message($"All messages processed successfully. Total processed messages generated: {totalProcessedMessages}");
        Log.Message($"Total phrases generated: {totalGeneratedPhrases}");

        string outputFilename = jsonDataFilename.Replace(".json", "_ophrase.json");
        DataLoader.Save(messageList, outputFilename);
        Log.Message($"Updated message list saved to {outputFilename}");
    }

    protected override async Task<string[]> Generate(string userContent, TimeSpan timeout)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            using (var cts = new CancellationTokenSource(timeout))
            {
                Log.Message($"Starting paraphrase task for user content: {userContent}");
                string[] result = await Ophrase.Instance.Paraphrase(userContent);
                Log.Message($"Paraphrase task completed for user content: {userContent}");
                return result.Select(StringUtilities.ScrubResponse).ToArray();
            }
        }
        catch (OperationCanceledException)
        {
            Log.Message($"Paraphrase task for user content '{userContent}' timed out. Retrying...");
            throw;
        }
        catch (Exception ex)
        {
            Log.Message($"Exception occurred during paraphrase task for user content '{userContent}': {ex.Message}. Retrying...");
            throw;
        }
        finally
        {
            stopwatch.Stop();
            Log.Message($"Paraphrase task for user content '{userContent}' completed in {stopwatch.ElapsedMilliseconds} ms.");
        }
    }
}
