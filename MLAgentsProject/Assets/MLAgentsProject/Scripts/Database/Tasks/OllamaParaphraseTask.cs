using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class OllamaParaphraseTask
{
    public async Task ProcessMessages(MessageList messageList, string jsonDataFilename)
    {
        int totalParaphrasedMessages = 0;

        foreach (var message in messageList.training_data)
        {
            var userContent = message.turns.First(turn => turn.role == "User").message;
            try
            {
                Log.Message($"Processing user content: {userContent}");

                var paraphrasedResponses = await ExecuteWithTimeout(userContent, TimeSpan.FromSeconds(30));
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
            catch (Exception ex)
            {
                Log.Error($"Exception occurred while processing user content '{userContent}': {ex.Message}");
            }
        }

        Log.Message($"All messages processed successfully. Total paraphrased messages generated: {totalParaphrasedMessages}");

        string outputFilename = jsonDataFilename.Replace(".json", "_ophrase.json");
        DataLoader.Save(messageList, outputFilename);
        Log.Message($"Updated message list saved to {outputFilename}");
    }

    private async Task<string[]> ExecuteWithTimeout(string userContent, TimeSpan timeout)
    {
        using (var cts = new CancellationTokenSource(timeout))
        {
            try
            {
                return await Execute(userContent, cts.Token);
            }
            catch (OperationCanceledException)
            {
                Log.Error($"Paraphrase task for user content '{userContent}' timed out.");
                return new string[0];
            }
        }
    }

    public async Task<string[]> Execute(string userContent, CancellationToken cancellationToken)
    {
        try
        {
            Log.Message($"Starting paraphrase task for user content: {userContent}");
            string[] result = await Ophrase.Instance.Paraphrase(userContent);
            Log.Message($"Paraphrase task completed for user content: {userContent}");
            return result;
        }
        catch (Exception ex)
        {
            Log.Error($"Exception occurred during paraphrase task for user content '{userContent}': {ex.Message}");
            return new string[0];
        }
    }
}
