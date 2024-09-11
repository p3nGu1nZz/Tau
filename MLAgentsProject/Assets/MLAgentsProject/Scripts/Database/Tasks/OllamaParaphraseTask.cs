using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class OllamaParaphraseTask : BaseTask<OllamaParaphraseTask>
{
    private static readonly int MaxConcurrency = 2;
    private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(MaxConcurrency);

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
        InitializeCounters();

        var newMessagesList = new List<Message>();
        var errorMessageList = new List<Message>();
        int totalMessages = messageList.training_data.Count;

        Log.Message($"Starting to process {totalMessages} user contents from {jsonDataFilename}...");

        var stopwatch = Stopwatch.StartNew();
        var tasks = CreateTasks(messageList, newMessagesList, errorMessageList, totalMessages);

        await HandleTasksCompletion(tasks);
        stopwatch.Stop();

        LogProcessingCompletion(stopwatch, messageList, newMessagesList, jsonDataFilename, errorMessageList);
    }

    private List<Task> CreateTasks(MessageList messageList, List<Message> newMessagesList, List<Message> errorMessageList, int totalMessages)
    {
        var tasks = new List<Task>();
        for (int i = 0; i < totalMessages; i++)
        {
            var message = messageList.training_data[i];
            var userContent = GetUserContent(message);
            tasks.Add(ProcessUserContent(userContent, message, newMessagesList, errorMessageList, i, totalMessages));
        }
        return tasks;
    }

    private async Task ProcessUserContent(string userContent, Message message, List<Message> newMessagesList, List<Message> errorMessageList, int index, int totalMessages)
    {
        await Semaphore.WaitAsync(CancellationTokenSource.Token);
        try
        {
            Log.Message($"Processing user content: {userContent} ({index + 1} of {totalMessages})");

            var responses = await Execute(userContent, TimeSpan.FromSeconds(30), 10);
            if (responses.Length == 0 || responses.Any(response => response.StartsWith("Error")))
            {
                throw new Exception($"Failed to generate valid responses for user content: {userContent}");
            }

            AddNewMessages(message, responses, newMessagesList);
            UpdateCounters(responses.Length, newMessagesList.Count);

            Log.Message($"Generated {responses.Length} responses for user content: {userContent}. Completed {index + 1} of {totalMessages} tasks.");
        }
        catch (Exception ex)
        {
            Log.Message($"Error processing user content '{userContent}': {ex.Message}");
            AddErrorMessage(message, errorMessageList);
        }
        finally
        {
            Semaphore.Release();
        }
    }

    private void AddNewMessages(Message message, string[] responses, List<Message> newMessagesList)
    {
        var newMessages = CreateNewMessages(message, responses);
        lock (newMessagesList)
        {
            newMessagesList.AddRange(newMessages);
        }
    }

    private void AddErrorMessage(Message message, List<Message> errorMessageList)
    {
        lock (errorMessageList)
        {
            errorMessageList.Add(message);
        }
        _counters.AddOrUpdate("totalErrorMessages", 1, (key, oldValue) => oldValue + 1);
    }

    private void UpdateCounters(int generatedPhrases, int processedMessages)
    {
        _counters.AddOrUpdate("totalProcessedMessages", processedMessages, (key, oldValue) => oldValue + processedMessages);
        _counters.AddOrUpdate("totalGeneratedPhrases", generatedPhrases, (key, oldValue) => oldValue + generatedPhrases);
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
