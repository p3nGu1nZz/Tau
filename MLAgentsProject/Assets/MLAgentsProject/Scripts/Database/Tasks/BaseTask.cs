using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public abstract class BaseTask<T, TResult> : ITask<TResult> where T : BaseTask<T, TResult>
{
    protected readonly ConcurrentDictionary<string, int> _counters = new ConcurrentDictionary<string, int>();
    protected CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
    protected static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(8);

    public abstract Task Process(MessageList messageList, string jsonDataFilename);
    public abstract Task<List<TResult>> Generate(string userContent, string agentContent, TimeSpan timeout);

    public async Task<List<TResult>> Execute(string userContent, string agentContent, TimeSpan timeout, int maxRetries = 1, int delay = 1) =>
        await TaskUtilities.Execute(t => Generate(userContent, agentContent, t), userContent, timeout, maxRetries, delay);

    public virtual string GetUserContent(Message message) => TaskUtilities.GetUserContent(message);

    public virtual string GetAgentContent(Message message) => TaskUtilities.GetAgentContent(message);

    public virtual List<Message> CreateNewMessages(Message originalMessage, List<string> responses) =>
        TaskUtilities.CreateNewMessages(originalMessage, responses);

    public Message GetMessage(MessageList messageList, int index) => messageList.training_data[index];

    public void SaveMessages(MessageList messageList, string jsonDataFilename, string suffix) =>
        TaskUtilities.SaveMessages(messageList, jsonDataFilename, suffix);

    public void SaveErrorMessages(List<Message> errorMessageList, string jsonDataFilename, int totalErrorMessages, string suffix) =>
        TaskUtilities.SaveErrorMessages(errorMessageList, jsonDataFilename, totalErrorMessages, suffix);

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

    public void LogProcessingCompletion(Stopwatch stopwatch, MessageList messageList, List<Message> newMessagesList, string jsonDataFilename, List<Message> errorMessageList, string suffix)
    {
        double elapsedMinutes = stopwatch.Elapsed.TotalMinutes;
        double paraphrasesPerMinute = _counters["totalGeneratedPhrases"] / elapsedMinutes;

        Log.Message($"Processing completed in {stopwatch.ElapsedMilliseconds} ms. Total paraphrases generated per minute: {paraphrasesPerMinute:F2}");

        lock (messageList.training_data)
        {
            messageList.training_data.AddRange(newMessagesList);
        }

        Log.Message($"All messages processed successfully. Total processed messages generated: {_counters["totalProcessedMessages"]}. Total phrases generated: {_counters["totalGeneratedPhrases"]}");

        SaveMessages(messageList, jsonDataFilename, suffix);
        SaveErrorMessages(errorMessageList, jsonDataFilename, _counters["totalErrorMessages"], suffix.Replace(".json","") + "_error.json");
    }

    public virtual List<Task> CreateTasks(MessageList messageList, List<Message> newMessagesList, List<Message> errorMessageList, int totalMessages)
    {
        return new List<Task>();
    }

    public virtual async Task ProcessContent(string userContent, string agentContent, Message message, List<Message> newMessagesList, List<Message> errorMessageList, int index, int totalMessages)
    {
        await Task.CompletedTask;
    }

    public void ValidateResponses(List<string> responses, string userContent) => TaskUtilities.ValidateResponses(responses, userContent);

    public void AddNewMessages(Message message, List<string> responses, List<Message> newMessagesList) =>
        TaskUtilities.AddNewMessages(message, responses, newMessagesList);

    public void AddErrorMessage(Message message, List<Message> errorMessageList) =>
        TaskUtilities.AddErrorMessage(message, errorMessageList, _counters);

    public void UpdateCounters(int generatedPhrases, int processedMessages) =>
        TaskUtilities.UpdateCounters(generatedPhrases, processedMessages, _counters);

    public virtual void RemoveMessages(MessageList messageList, List<Message> responses) { }
}
