using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class OllamaProofTask : BaseTask<OllamaProofTask, Response>
{
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
        LogProcessingCompletion(stopwatch, messageList, newMessagesList, jsonDataFilename, errorMessageList, "_oproof.json");
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

    public override List<Task> CreateTasks(MessageList messageList, List<Message> newMessagesList, List<Message> errorMessageList, int totalMessages)
    {
        Log.Message("OllamaProofTask: Creating tasks for proof processing.");
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
            Log.Message($"OllamaProofTask: Processing user content: {userContent} ({index + 1} of {totalMessages})");

            var responses = await Execute(userContent, agentContent, TimeSpan.FromSeconds(30), 10);
            ValidateResponses(responses.Select(r => r.ToString()).ToList(), userContent);

            // New log line specific to proof task
            Log.Message("OllamaProofTask: Validation completed for proof task.");

            AddNewMessages(message, responses.Select(r => r.ToString()).ToList(), newMessagesList);
            UpdateCounters(responses.Count, newMessagesList.Count);

            Log.Message($"OllamaProofTask: Generated {responses.Count} responses for user content: {userContent}. Completed {index + 1} of {totalMessages} tasks.");
        }
        catch (Exception ex)
        {
            Log.Message($"OllamaProofTask: Error processing user content '{userContent}': {ex.Message}");
            AddErrorMessage(message, errorMessageList);
        }
        finally
        {
            Semaphore.Release();
        }
    }
}
