using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class OllamaParaphraseTask : BaseTask<OllamaParaphraseTask, string>
{
    public OllamaParaphraseTask() => Application.quitting += OnApplicationQuit;

    private void OnApplicationQuit() => CancellationTokenSource.Cancel();

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
        LogProcessingCompletion(stopwatch, messageList, newMessagesList, jsonDataFilename, errorMessageList, "_ophrase.json");
    }

    public override async Task<List<string>> Generate(string userContent, string agentContent, TimeSpan timeout)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            using (var cts = new CancellationTokenSource(timeout))
            {
                Log.Message($"Starting paraphrase task for user content: {userContent}");
                List<string> result = await Ophrase.Instance.Paraphrase(userContent);
                Log.Message($"Paraphrase task completed for user content: {userContent}");
                return result.Select(StringUtilities.ScrubResponse).ToList();
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

    public override List<Task> CreateTasks(MessageList messageList, List<Message> newMessagesList, List<Message> errorMessageList, int totalMessages)
    {
        return Enumerable.Range(0, totalMessages)
                         .Select(i =>
                         {
                             var message = GetMessage(messageList, i);
                             var userContent = GetUserContent(message);
                             var agentContent = GetAgentContent(message);
                             return ProcessContent(userContent, agentContent, message, newMessagesList, errorMessageList, i, totalMessages);
                         })
                         .ToList();
    }

    public override async Task ProcessContent(string userContent, string agentContent, Message message, List<Message> newMessagesList, List<Message> errorMessageList, int index, int totalMessages)
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
}
