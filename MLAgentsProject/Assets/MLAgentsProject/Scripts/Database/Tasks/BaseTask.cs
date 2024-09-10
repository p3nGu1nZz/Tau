using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public abstract class BaseTask<T> where T : BaseTask<T>
{
    public abstract Task Process(MessageList messageList, string jsonDataFilename);

    protected abstract Task<string[]> Generate(string userContent, TimeSpan timeout);

    protected async Task<string[]> Execute(string userContent, TimeSpan timeout, int maxRetries = 1)
    {
        int attempt = 0;
        while (attempt < maxRetries)
        {
            Log.Message($"Attempt {attempt + 1} of {maxRetries} for user content: {userContent}");
            try
            {
                using (var cts = new CancellationTokenSource(timeout))
                {
                    Log.Message($"Starting task for user content: {userContent}");
                    string[] result = await Generate(userContent, timeout);
                    Log.Message($"Task completed for user content: {userContent}");
                    return result;
                }
            }
            catch (OperationCanceledException)
            {
                Log.Error($"Task for user content '{userContent}' timed out. Attempt {attempt + 1} of {maxRetries}");
                attempt++;
            }
            catch (Exception ex)
            {
                Log.Error($"Attempt {attempt + 1} failed for user content '{userContent}': {ex.Message}");
                attempt++;
            }

            Log.Message($"Retrying task for user content: {userContent}. Attempt {attempt + 1} of {maxRetries}");
        }

        Log.Error($"Max retries reached for user content '{userContent}'.");
        return new string[] { "Error: Unable to generate responses." };
    }

    protected virtual string ScrubResponse(string response)
    {
        // Implement scrubbing logic to remove invalid characters
        return response.Replace("\n", "").Replace("\r", "").Replace("\\", "").Replace("\"", "\\\"");
    }
}
