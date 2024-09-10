using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class OllamaParaphraseTask : BaseTask<OllamaParaphraseTask>
{
    public override async Task Process(MessageList messageList, string jsonDataFilename)
    {
        int totalProcessedMessages = 0;
        int totalGeneratedPhrases = 0;
        var newMessagesList = new List<Message>();
        int totalMessages = messageList.training_data.Count;
        int timeoutLength = 30;
        int retryAmount = 5;

        Log.Message($"Starting to process {totalMessages} user contents from {jsonDataFilename}...");

        for (int i = 0; i < totalMessages; i++)
        {
            try
            {
                var message = messageList.training_data[i];
                var userContent = GetUserContent(message);
                Log.Message($"Processing user content: {userContent} ({i + 1} of {totalMessages})");

                var responses = await Execute(userContent, TimeSpan.FromSeconds(timeoutLength), retryAmount);
                if (responses.Length == 0 || responses.Any(response => response.StartsWith("Error")))
                {
                    Log.Error($"Failed to generate valid responses for user content: {userContent}");
                    continue; // Skip adding this message to the newMessagesList
                }

                var newMessages = CreateNewMessages(message, responses);
                newMessagesList.AddRange(newMessages);
                totalProcessedMessages += newMessages.Count;
                totalGeneratedPhrases += responses.Length;

                Log.Message($"Generated {newMessages.Count} responses for user content: {userContent}");
                Log.Message($"Completed {i + 1} of {totalMessages} tasks.");
                Log.Message($"Total phrases generated so far: {totalGeneratedPhrases}");
            }
            catch (Exception ex)
            {
                Log.Error($"Exception occurred while processing user content: {ex.Message}");
                Log.Error("Exiting task due to error.");
                throw;
            }
        }

        lock (messageList.training_data)
        {
            messageList.training_data.AddRange(newMessagesList);
        }

        Log.Message($"All messages processed successfully. Total processed messages generated: {totalProcessedMessages}");
        Log.Message($"Total phrases generated: {totalGeneratedPhrases}");

        string outputFilename = jsonDataFilename.Replace(".json", "_ophrase.json");
        DataLoader.Save(messageList, outputFilename);
        Log.Message($"Updated message list saved to {outputFilename}");
    }

    protected override async Task<string[]> Generate(string userContent, TimeSpan timeout)
    {
        try
        {
            using (var cts = new CancellationTokenSource(timeout))
            {
                Log.Message($"Starting paraphrase task for user content: {userContent}");
                string[] result = await Ophrase.Instance.Paraphrase(userContent);
                Log.Message($"Paraphrase task completed for user content: {userContent}");
                return result.Select(StringUtilities.ScrubResponse).ToArray();
            }
        }
        catch (OperationCanceledException)
        {
            Log.Error($"Paraphrase task for user content '{userContent}' timed out.");
            throw;
        }
        catch (Exception ex)
        {
            Log.Error($"Exception occurred during paraphrase task for user content '{userContent}': {ex.Message}");
            throw;
        }
    }

    protected virtual string GetUserContent(Message message)
    {
        return message.turns.First(turn => turn.role == "User").message;
    }

    protected virtual List<Message> CreateNewMessages(Message originalMessage, string[] responses)
    {
        var agentResponse = originalMessage.turns.Last(turn => turn.role == "Agent").message;

        return responses.Select(response => new Message
        {
            domain = originalMessage.domain,
            context = originalMessage.context,
            system = originalMessage.system,
            turns = new List<Turn>
            {
                new() { role = "User", message = response },
                new() { role = "Agent", message = agentResponse }
            }
        }).ToList();
    }
}
