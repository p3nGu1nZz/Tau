using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class OllamaParaphraseTask
{
    public async Task ProcessMessages(MessageList messageList, string jsonDataFilename)
    {
        int totalParaphrasedMessages = 0;
        var newMessagesList = new List<Message>();
        int totalMessages = messageList.training_data.Count;
        int timeoutLength = 30;

        Log.Message($"Starting to process {totalMessages} user contents from {jsonDataFilename}...");

        for (int i = 0; i < totalMessages; i++)
        {
            try
            {
                var message = messageList.training_data[i];
                var userContent = message.turns.First(turn => turn.role == "User").message;
                Log.Message($"Processing user content: {userContent} ({i + 1} of {totalMessages})");

                var paraphrasedResponses = await Execute(userContent, TimeSpan.FromSeconds(timeoutLength));
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

                newMessagesList.AddRange(newMessages);
                totalParaphrasedMessages += newMessages.Count;

                Log.Message($"Generated {newMessages.Count} paraphrased messages for user content: {userContent}");
                Log.Message($"Completed {i + 1} of {totalMessages} paraphrase tasks.");
            }
            catch (Exception ex)
            {
                Log.Error($"Exception occurred while processing user content: {ex.Message}");
            }
        }

        lock (messageList.training_data)
        {
            messageList.training_data.AddRange(newMessagesList);
        }

        Log.Message($"All messages processed successfully. Total paraphrased messages generated: {totalParaphrasedMessages}");

        string outputFilename = jsonDataFilename.Replace(".json", "_ophrase.json");
        DataLoader.Save(messageList, outputFilename);
        Log.Message($"Updated message list saved to {outputFilename}");
    }

    private async Task<string[]> Execute(string userContent, TimeSpan timeout)
    {
        try
        {
            using (var cts = new CancellationTokenSource(timeout))
            {
                Log.Message($"Starting paraphrase task for user content: {userContent}");
                string[] result = await Ophrase.Instance.Paraphrase(userContent);
                Log.Message($"Paraphrase task completed for user content: {userContent}");
                return result;
            }
        }
        catch (OperationCanceledException)
        {
            Log.Error($"Paraphrase task for user content '{userContent}' timed out.");
            return new string[0];
        }
        catch (Exception ex)
        {
            Log.Error($"Exception occurred during paraphrase task for user content '{userContent}': {ex.Message}");
            return new string[0];
        }
    }
}
