using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public static class TaskUtilities
{
    public static async Task<string[]> ExecuteWithRetries(Func<TimeSpan, Task<string[]>> generateFunc, string userContent, TimeSpan timeout, int maxRetries = 1, int delay = 1000)
    {
        int attempt = 0;
        await Task.Delay(delay);

        while (attempt < maxRetries)
        {
            Log.Message($"Attempt {attempt + 1} of {maxRetries} for user content: {userContent}");
            try
            {
                using (var cts = new CancellationTokenSource(timeout))
                {
                    Log.Message($"Starting task for user content: {userContent}");
                    string[] result = await generateFunc(timeout);
                    Log.Message($"Task completed for user content: {userContent}");
                    return result;
                }
            }
            catch (OperationCanceledException)
            {
                Log.Message($"Task for user content '{userContent}' timed out. Retrying... Attempt {attempt + 1} of {maxRetries}");
                attempt++;
            }
            catch (Exception ex)
            {
                if (attempt + 1 == maxRetries)
                {
                    throw new Exception($"Max retries ({attempt + 1}) reached for user content '{userContent}': {ex.Message}");
                }
                else
                {
                    Log.Message($"Attempt {attempt + 1} failed for user content '{userContent}': {ex.Message}. Retrying...");
                }
                attempt++;
            }

            await Task.Delay(delay);
        }

        return new string[] { "Error: Unable to generate responses." };
    }

    public static string GetUserContent(Message message)
    {
        return message.turns.First(turn => turn.role == "User").message;
    }

    public static List<Message> CreateNewMessages(Message originalMessage, string[] responses)
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

    public static void SaveMessages(MessageList messageList, string jsonDataFilename, string suffix)
    {
        string outputFilename = jsonDataFilename.Replace(".json", suffix);
        DataLoader.Save(messageList, outputFilename);
        Log.Message($"Messages saved to {outputFilename}");
    }

    public static void SaveErrorMessages(List<Message> errorMessageList, string jsonDataFilename, int totalErrorMessages)
    {
        if (errorMessageList.Count > 0)
        {
            var errorMessageListWrapper = new MessageList { training_data = errorMessageList };
            SaveMessages(errorMessageListWrapper, jsonDataFilename, "_ophrase_error.json");
            Log.Message($"Total error messages generated: {totalErrorMessages}");
        }
    }
}
