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
    protected static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(2);

    public abstract Task Process(MessageList messageList, string jsonDataFilename);
    public abstract Task<List<TResult>> Generate(string userContent, string agentContent, TimeSpan timeout);

    public async Task<List<TResult>> Execute(string userContent, string agentContent, TimeSpan timeout, int maxRetries = 1, int delay = 1000) =>
        await TaskUtilities.Execute(t => Generate(userContent, agentContent, t), userContent, timeout, maxRetries, delay);

    public virtual string GetUserContent(Message message) => TaskUtilities.GetUserContent(message);

    public virtual string GetAgentContent(Message message) => TaskUtilities.GetAgentContent(message);

    public virtual List<Message> CreateNewMessages(Message originalMessage, List<string> responses) =>
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
                  .Select(i =>
                  {
                      var message = GetMessage(messageList, i);
                      var userContent = GetUserContent(message);
                      var agentContent = GetAgentContent(message);
                      return ProcessUserContent(userContent, agentContent, message, newMessagesList, errorMessageList, i, totalMessages);
                  })
                  .ToList();

    public async Task ProcessUserContent(string userContent, string agentContent, Message message, List<Message> newMessagesList, List<Message> errorMessageList, int index, int totalMessages)
    {
        await Semaphore.WaitAsync(CancellationTokenSource.Token);
        try
        {
            Log.Message($"Processing user content: {userContent} ({index + 1} of {totalMessages})");

            var responses = await Execute(userContent, agentContent, TimeSpan.FromSeconds(30), 10);
            ValidateResponses(responses.Select(r => r.ToString()).ToList(), userContent);

            AddNewMessages(message, responses.Select(r => r.ToString()).ToList(), newMessagesList);
            UpdateCounters(responses.Count, newMessagesList.Count);

            Log.Message($"Generated {responses.Count} responses for user content: {userContent}. Completed {index + 1} of {totalMessages} tasks.");
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

    public void ValidateResponses(List<string> responses, string userContent) => TaskUtilities.ValidateResponses(responses, userContent);

    public void AddNewMessages(Message message, List<string> responses, List<Message> newMessagesList) =>
        TaskUtilities.AddNewMessages(message, responses, newMessagesList);

    public void AddErrorMessage(Message message, List<Message> errorMessageList) =>
        TaskUtilities.AddErrorMessage(message, errorMessageList, _counters);

    public void UpdateCounters(int generatedPhrases, int processedMessages) =>
        TaskUtilities.UpdateCounters(generatedPhrases, processedMessages, _counters);
}
