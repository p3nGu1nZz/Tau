using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public abstract class BaseTask<T> where T : BaseTask<T>
{
    protected readonly ConcurrentDictionary<string, int> _counters = new ConcurrentDictionary<string, int>();
    protected CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

    public abstract Task Process(MessageList messageList, string jsonDataFilename);

    protected abstract Task<string[]> Generate(string userContent, TimeSpan timeout);

    protected async Task<string[]> Execute(string userContent, TimeSpan timeout, int maxRetries = 1, int delay = 1000)
    {
        return await TaskUtilities.ExecuteWithRetries(t => Generate(userContent, t), userContent, timeout, maxRetries, delay);
    }

    protected virtual string GetUserContent(Message message)
    {
        return TaskUtilities.GetUserContent(message);
    }

    protected virtual List<Message> CreateNewMessages(Message originalMessage, string[] responses)
    {
        return TaskUtilities.CreateNewMessages(originalMessage, responses);
    }

    protected void SaveMessages(MessageList messageList, string jsonDataFilename, string suffix)
    {
        TaskUtilities.SaveMessages(messageList, jsonDataFilename, suffix);
    }

    protected void SaveErrorMessages(List<Message> errorMessageList, string jsonDataFilename, int totalErrorMessages)
    {
        TaskUtilities.SaveErrorMessages(errorMessageList, jsonDataFilename, totalErrorMessages);
    }

    protected void InitializeCounters()
    {
        _counters["totalProcessedMessages"] = 0;
        _counters["totalGeneratedPhrases"] = 0;
        _counters["totalErrorMessages"] = 0;
    }

    protected async Task HandleTasksCompletion(List<Task> tasks)
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

    protected void LogProcessingCompletion(Stopwatch stopwatch, MessageList messageList, List<Message> newMessagesList, string jsonDataFilename, List<Message> errorMessageList)
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
}
