using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public static class DataAuditor
{
    private static SemaphoreSlim _semaphore = new(EmbeddingUtilities.MaxConcurrentJobs);
    private static CancellationTokenSource _cts = new();
    public static int TaskStartDelay { get; set; } = 1;

    public static async Task Audit(string fileName)
    {
        if (!Database.Instance.IsLoaded())
        {
            Log.Error("No database loaded. Please load a database or training data.");
            return;
        }

        Stopwatch stopwatch = Stopwatch.StartNew();
        HashSet<string> missingEmbeddings = new();
        Log.Message("Starting audit process...");

        LoadTrainingData(fileName, missingEmbeddings);
        await RetryMissingEmbeddings(missingEmbeddings);
        FinalValidation(fileName);

        stopwatch.Stop();
        GenerateAuditReport(fileName, missingEmbeddings, stopwatch.ElapsedMilliseconds);
    }

    private static void LoadTrainingData(string fileName, HashSet<string> missingEmbeddings)
    {
        try
        {
            Log.Message($"Loading training data from file: '{fileName}'.");
            var messageList = DataLoader.Load(fileName);
            Log.Message($"Loaded {messageList.training_data.Count} messages from training data.");

            foreach (var message in messageList.training_data)
            {
                ProcessMessageTurns(message.turns, missingEmbeddings);
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to load training data from file '{fileName}': {ex.Message}");
        }
    }

    private static void ProcessMessageTurns(List<Turn> turns, HashSet<string> missingEmbeddings)
    {
        try
        {
            TrainingDataFactory.CheckTurns(turns);

            for (int i = 0; i < turns.Count - 1; i += 2)
            {
                var userTurn = turns[i];
                var agentTurn = turns[i + 1];

                CheckAndAddMissingEmbedding(userTurn.message.ToLower(), TableNames.TrainingData, missingEmbeddings);
                CheckAndAddMissingEmbedding(agentTurn.message.ToLower(), TableNames.Tokens, missingEmbeddings);
            }
        }
        catch (InvalidDataException ex)
        {
            Log.Error($"Error processing message: {ex.Message}");
        }
    }

    private static void CheckAndAddMissingEmbedding(string token, string tableName, HashSet<string> missingEmbeddings)
    {
        if (!TryFindEmbedding(token, tableName))
        {
            Log.Message($"Missing embedding for token: '{token}'.");
            missingEmbeddings.Add(token);
        }
    }

    private static bool TryFindEmbedding(string token, string tableName)
    {
        try
        {
            var embedding = TrainingDataFactory.FindEmbedding(token, tableName);
            return true;
        }
        catch (KeyNotFoundException)
        {
            return false;
        }
    }

    private static async Task RetryMissingEmbeddings(HashSet<string> missingEmbeddings)
    {
        int retryCount = 0, maxRetries = 3;
        while (missingEmbeddings.Count > 0 && retryCount < maxRetries)
        {
            retryCount++;
            Log.Message($"Retrying to create missing embeddings. Attempt {retryCount}. Missing embeddings count: {missingEmbeddings.Count}");
            missingEmbeddings = new HashSet<string>(await AttemptToCreateEmbeddings(missingEmbeddings));
        }
    }

    private static async Task<HashSet<string>> AttemptToCreateEmbeddings(HashSet<string> missingEmbeddings)
    {
        HashSet<string> newlyMissingEmbeddings = new();
        var tasks = new List<Task>();

        int i = 0;
        foreach (var embedding in missingEmbeddings)
        {
            await _semaphore.WaitAsync(_cts.Token);

            int currentIndex = i++;
            tasks.Add(Task.Run(async () =>
            {
                await CreateEmbeddingTask(embedding, currentIndex, missingEmbeddings.Count, newlyMissingEmbeddings);
            }, _cts.Token));

            await Task.Delay(TaskStartDelay, _cts.Token);
        }

        await Task.WhenAll(tasks);
        return newlyMissingEmbeddings;
    }

    private static async Task CreateEmbeddingTask(string embedding, int currentIndex, int totalCount, HashSet<string> newlyMissingEmbeddings)
    {
        try
        {
            Log.Message($"Attempting to create embedding for '{embedding}' ({currentIndex + 1}/{totalCount})");
            var type = EmbeddingUtilities.DetermineEmbeddingType(embedding);
            double[] generatedEmbedding = await GenerateEmbeddingTask.Execute(embedding, type);

            if (generatedEmbedding != null)
            {
                Database.Instance.Add(embedding, generatedEmbedding, TableNames.Tokens, type);
                Log.Message($"Successfully created embedding for '{embedding}' ({currentIndex + 1}/{totalCount}).");
            }
            else
            {
                Log.Error($"Failed to generate embedding for '{embedding}' ({currentIndex + 1}/{totalCount}).");
                newlyMissingEmbeddings.Add(embedding);
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Error generating embedding for '{embedding}' ({currentIndex + 1}/{totalCount}): {ex.Message}");
            newlyMissingEmbeddings.Add(embedding);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private static void FinalValidation(string fileName)
    {
        try
        {
            Log.Message("Performing final validation...");
            var trainingData = TrainingDataFactory.CreateTrainingData(fileName);
            if (trainingData == null || trainingData.Count == 0)
            {
                Log.Warning("Final validation found no training data.");
            }
            else
            {
                Log.Message($"Final validation completed successfully. All embeddings are present. Total training data count: {trainingData.Count}");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Final validation failed: {ex.Message}");
        }
    }

    private static void GenerateAuditReport(string fileName, HashSet<string> missingEmbeddings, long elapsedMilliseconds)
    {
        var audit = new Audit
        {
            version = Constants.Version,
            model_name = Constants.ModelName,
            organization = Constants.Organization,
            total_messages = missingEmbeddings.Count,
            missing_embeddings = missingEmbeddings.Count,
            created_embeddings = missingEmbeddings.Count - missingEmbeddings.Count,
            missing_tokens = new List<string>(missingEmbeddings)
        };

        string auditFileName = Path.GetFileNameWithoutExtension(fileName) + "_audit.json";
        string auditFilePath = DataUtilities.GetFilePath(auditFileName);
        DataUtilities.SaveToFile(audit, auditFilePath);

        Log.Message($"Audit completed in {elapsedMilliseconds} ms. " +
                    $"Embeddings found: {missingEmbeddings.Count}, Embeddings added: {missingEmbeddings.Count - missingEmbeddings.Count}");
    }
}
