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
                Log.Message($"Task for user content '{userContent}' timed out. Retrying... Attempt {attempt + 1} of {maxRetries}");
                attempt++;
            }
            catch (Exception ex)
            {
                if (attempt + 1 == maxRetries)
                {
                    Log.Error($"Attempt {attempt + 1} failed for user content '{userContent}': {ex.Message}");
                }
                else
                {
                    Log.Message($"Attempt {attempt + 1} failed for user content '{userContent}': {ex.Message}. Retrying...");
                }
                attempt++;
            }
        }

        Log.Error($"Max retries reached for user content '{userContent}'.");
        return new string[] { "Error: Unable to generate responses." };
    }
}
