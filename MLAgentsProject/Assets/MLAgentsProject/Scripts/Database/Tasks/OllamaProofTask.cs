using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class OllamaProofTask : BaseTask<OllamaProofTask, Response>
{
    public OllamaProofTask() => Application.quitting += OnApplicationQuit;

    private void OnApplicationQuit() => CancellationTokenSource.Cancel();

    public override async Task Process(MessageList messageList, string jsonDataFilename)
    {
        InitializeCounters();
        var newMessagesList = new List<Message>();
        var errorMessageList = new List<Message>();
        int totalMessages = messageList.training_data.Count;
        Log.Message($"Starting to process {totalMessages} user and agent contents from {jsonDataFilename}...");
        var stopwatch = Stopwatch.StartNew();
        var tasks = CreateTasks(messageList, newMessagesList, errorMessageList, totalMessages);
        await HandleTasksCompletion(tasks);
        stopwatch.Stop();
        LogProcessingCompletion(stopwatch, messageList, newMessagesList, jsonDataFilename, errorMessageList);
    }

    public override async Task<List<Response>> Generate(string userContent, string agentContent, TimeSpan timeout)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            using (var cts = new CancellationTokenSource(timeout))
            {
                Log.Message($"Starting proof task for user content: {userContent}");
                List<Response> result = await Oproof.Instance.Proof(userContent, agentContent);
                Log.Message($"Proof task completed for user content: {userContent}");
                return result;
            }
        }
        catch (OperationCanceledException)
        {
            Log.Message($"Proof task for user content '{userContent}' timed out. Retrying...");
            throw;
        }
        catch (Exception ex)
        {
            Log.Message($"Exception occurred during proof task for user content '{userContent}': {ex.Message}. Retrying...");
            throw;
        }
        finally
        {
            stopwatch.Stop();
            Log.Message($"Proof task for user content '{userContent}' completed in {stopwatch.ElapsedMilliseconds} ms.");
        }
    }
}
