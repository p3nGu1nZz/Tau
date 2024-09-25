using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class TableBuilder
{
    private EmbeddingStorage _embeddingStorage;
    private DatabaseManager _tableManager;
    public int TaskStartDelay { get; set; } = 250;
    private CancellationTokenSource _cts;

    public TableBuilder(EmbeddingStorage embeddingStorage, DatabaseManager tableManager)
    {
        _embeddingStorage = embeddingStorage;
        _tableManager = tableManager;
        _cts = new CancellationTokenSource();

        // Subscribe to the application quitting event
        Application.quitting += OnApplicationQuit;
    }

    private void OnApplicationQuit()
    {
        _cts.Cancel();
    }

    public async Task BuildTable(List<string> tokens, string tableName, bool isVocabulary = false)
    {
        var tables = _tableManager.GetTables();
        if (!tables.ContainsKey(tableName))
        {
            _tableManager.CreateTable(tableName);
        }

        // Reset embeddings before starting the task
        _embeddingStorage.ResetEmbeddings();

        Stopwatch stopwatch = Stopwatch.StartNew();

        // Include reserved words in the total token count only if it's the vocabulary table
        int reservedWordsCount = isVocabulary ? Constants.ReservedWords.Length : 0;
        int totalTokens = tokens.Count + reservedWordsCount + 1;
        var tasks = new List<Task>();
        var semaphore = new SemaphoreSlim(EmbeddingUtilities.MaxConcurrentJobs);

        for (int i = 0; i < tokens.Count; i++)
        {
            string token = tokens[i];
            EmbeddingType type = EmbeddingType.Word;
            if (!isVocabulary)
            {
                type = EmbeddingUtilities.DetermineEmbeddingType(token);
            }

            if (_embeddingStorage.DoesTokenExist(token))
            {
                Log.Message($"Duplicate token '{token}' found, skipping.");
                continue;
            }

            await semaphore.WaitAsync(_cts.Token);

            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    Log.Message($"Processing token '{token}' ({i + 1}/{tokens.Count})");
                    var tokenStopwatch = Stopwatch.StartNew();
                    double[] embedding = await GenerateEmbeddingTask.Execute(token, type);
                    tokenStopwatch.Stop();

                    if (embedding != null)
                    {
                        _embeddingStorage.Add(token, embedding, tableName, type);
                        Log.Message($"Processed {i}/{tokens.Count} tokens in {tokenStopwatch.ElapsedMilliseconds} ms.");
                    }
                    else
                    {
                        Log.Error($"Failed to generate embedding for token '{token}'.");
                        _cts.Cancel();
                    }
                }
                catch (OperationCanceledException)
                {
                    Log.Error("Operation canceled.");
                }
                finally
                {
                    semaphore.Release();
                }
            }, _cts.Token));

            await Task.Delay(TaskStartDelay, _cts.Token);
        }

        try
        {
            await Task.WhenAll(tasks);
            Log.Message($"All '{tokens.Count}' tokens processed successfully in {stopwatch.ElapsedMilliseconds:F2} ms.");
        }
        catch (OperationCanceledException)
        {
            Log.Error("Task execution canceled due to an error.");
        }

        stopwatch.Stop();
        double totalTimeMinutes = stopwatch.Elapsed.TotalMinutes;
        double tokensPerMinute = totalTokens / totalTimeMinutes;
        Log.Message($"Total time: {totalTimeMinutes:F2} minutes (Tokens per minute: {tokensPerMinute:F2})");

        Log.Message($"Table '{tableName}' built successfully with {tokens.Count} tokens processed.");
    }
}
