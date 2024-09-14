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
}
