using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public abstract class BaseTask<T> : ITask where T : BaseTask<T>
{
    protected readonly ConcurrentDictionary<string, int> _counters = new ConcurrentDictionary<string, int>();
    protected CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
    protected static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(2);

    public abstract Task Process(MessageList messageList, string jsonDataFilename);
    public abstract Task<string[]> Generate(string userContent, TimeSpan timeout);

    public async Task<string[]> Execute(string userContent, TimeSpan timeout, int maxRetries = 1, int delay = 1000) =>
        await TaskUtilities.ExecuteWithRetries(t => Generate(userContent, t), userContent, timeout, maxRetries, delay);

    public virtual string GetUserContent(Message message) => TaskUtilities.GetUserContent(message);

    public virtual List<Message> CreateNewMessages(Message originalMessage, string[] responses) =>
        TaskUtilities.CreateNewMessages(originalMessage, responses);

    public Message GetMessage(MessageList messageList, int index) => messageList.training_data[index];

    public void SaveMessages(MessageList messageList, string jsonDataFilename, string suffix) =>
        TaskUtilities.SaveMessages(messageList, jsonDataFilename, suffix);

    public void SaveErrorMessages(List<Message> errorMessageList, string jsonDataFilename, int totalErrorMessages) =>
        TaskUtilities.SaveErrorMessages(errorMessageList, jsonDataFilename, totalErrorMessages);

    public void InitializeCounters()
    {
        _counters["totalProcessedMessages"] = 0;
        _counters["totalGeneratedPhrases"] = 0;
        _counters["totalErrorMessages"] = 0;
    }

    public async Task HandleTasksCompletion(List<Task> tasks)
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

    public void LogProcessingCompletion(Stopwatch stopwatch, MessageList messageList, List<Message> newMessagesList, string jsonDataFilename, List<Message> errorMessageList)
    {
        double elapsedMinutes = stopwatch.Elapsed.TotalMinutes;
        double paraphrasesPerMinute = _counters["totalGeneratedPhrases"] / elapsedMinutes;

        Log.Message($"Processing completed in {stopwatch.ElapsedMilliseconds} ms. Total paraphrases generated per minute: {paraphrasesPerMinute:F2}");

        lock (messageList.training_data)
        {
            messageList.training_data.AddRange(newMessagesList);
        }

        Log.Message($"All messages processed successfully. Total processed messages generated: {_counters["totalProcessedMessages"]}. Total phrases generated: {_counters["totalGeneratedPhrases"]}");

        SaveMessages(messageList, jsonDataFilename, "_ophrase.json");
        SaveErrorMessages(errorMessageList, jsonDataFilename, _counters["totalErrorMessages"]);
    }

    public List<Task> CreateTasks(MessageList messageList, List<Message> newMessagesList, List<Message> errorMessageList, int totalMessages) =>
        Enumerable.Range(0, totalMessages)
                  .Select(i => ProcessUserContent(GetUserContent(GetMessage(messageList, i)), GetMessage(messageList, i), newMessagesList, errorMessageList, i, totalMessages))
                  .ToList();

    public async Task ProcessUserContent(string userContent, Message message, List<Message> newMessagesList, List<Message> errorMessageList, int index, int totalMessages)
    {
        await Semaphore.WaitAsync(CancellationTokenSource.Token);
        try
        {
            Log.Message($"Processing user content: {userContent} ({index + 1} of {totalMessages})");

            var responses = await Execute(userContent, TimeSpan.FromSeconds(30), 10);
            ValidateResponses(responses, userContent);

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

    public void ValidateResponses(string[] responses, string userContent) => TaskUtilities.ValidateResponses(responses, userContent);

    public void AddNewMessages(Message message, string[] responses, List<Message> newMessagesList) =>
        TaskUtilities.AddNewMessages(message, responses, newMessagesList);

    public void AddErrorMessage(Message message, List<Message> errorMessageList) =>
        TaskUtilities.AddErrorMessage(message, errorMessageList, _counters);

    public void UpdateCounters(int generatedPhrases, int processedMessages) =>
        TaskUtilities.UpdateCounters(generatedPhrases, processedMessages, _counters);
}
