using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
                Log.Message($"Generate: Starting proof task for user content: '{userContent}' with agent content: '{agentContent}'");
                List<Response> result = await Oproof.Instance.Proof(userContent, agentContent);

                // Log the result with all properties
                foreach (var response in result)
                {
                    Log.Message($"Generate: Proof task completed with response: " +
                                $"prompt='{response.prompt}', " +
                                $"response='{response.response}', " +
                                $"is_valid='{response.is_valid}', " +
                                $"domain='{response.domain}', " +
                                $"context='{response.context}', " +
                                $"reason='{response.reason ?? string.Empty}'");
                }

                return result;
            }
        }
        catch (OperationCanceledException)
        {
            Log.Message($"Generate: Proof task for user content '{userContent}' timed out. Retrying...");
            throw;
        }
        catch (Exception ex)
        {
            Log.Message($"Generate: Exception occurred during proof task for user content '{userContent}': {ex.Message}. Retrying...");
            throw;
        }
        finally
        {
            stopwatch.Stop();
            Log.Message($"Generate: Proof task for user content '{userContent}' completed in {stopwatch.ElapsedMilliseconds} ms.");
        }
    }
}
