using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class OllamaParaphraseTask
{
    private SemaphoreSlim _semaphore;
    private CancellationToken _cancellationToken;

    public OllamaParaphraseTask(SemaphoreSlim semaphore, CancellationToken cancellationToken)
    {
        _semaphore = semaphore;
        _cancellationToken = cancellationToken;
    }

    public async Task ProcessMessages(MessageList messageList, string jsonDataFilename)
    {
        var tasks = new List<Task>();
        int totalParaphrasedMessages = 0;

        foreach (var message in messageList.training_data)
        {
            tasks.Add(Task.Run(async () =>
            {
                await _semaphore.WaitAsync(_cancellationToken);
                try
                {
                    var userContent = message.turns.First(turn => turn.role == "User").message;
                    Log.Message($"Processing user content: {userContent}");

                    var paraphrasedResponses = await Execute(userContent);
                    var agentResponse = message.turns.Last(turn => turn.role == "Agent").message;

                    var newMessages = paraphrasedResponses.Select(response => new Message
                    {
                        domain = message.domain,
                        context = message.context,
                        system = message.system,
                        turns = new List<Turn>
                        {
                            new() { role = "User", message = response },
                            new() { role = "Agent", message = agentResponse }
                        }
                    }).ToList();

                    lock (messageList.training_data)
                    {
                        messageList.training_data.AddRange(newMessages);
                        totalParaphrasedMessages += newMessages.Count;
                    }

                    Log.Message($"Generated {newMessages.Count} paraphrased messages for user content: {userContent}");
                }
                finally
                {
                    _semaphore.Release();
                }
            }, _cancellationToken));
        }

        try
        {
            await Task.WhenAll(tasks);
            Log.Message($"All messages processed successfully. Total paraphrased messages generated: {totalParaphrasedMessages}");

            // Save the updated message list to a new JSON file
            string outputFilename = jsonDataFilename.Replace(".json", "_ophrase.json");
            DataLoader.Save(messageList, outputFilename);
            Log.Message($"Updated message list saved to {outputFilename}");
        }
        catch (OperationCanceledException)
        {
            Log.Error("Task execution canceled due to an error.");
        }
    }

    public async Task<string[]> Execute(string userContent)
    {
        await _semaphore.WaitAsync(_cancellationToken);
        try
        {
            string[] result = await Ophrase.Instance.Paraphrase(userContent);
            return result;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
