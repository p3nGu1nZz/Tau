using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using System.Linq;

public class TableBuilder
{
    private EmbeddingStorage _embeddingStorage;
    private DatabaseManager _tableManager;
    public int TaskStartDelay { get; set; } = 1;
    private CancellationTokenSource _cts;

    public TableBuilder(EmbeddingStorage embeddingStorage, DatabaseManager tableManager)
    {
        _embeddingStorage = embeddingStorage;
        _tableManager = tableManager;
        _cts = new CancellationTokenSource();

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

        _embeddingStorage.ResetEmbeddings();

        Stopwatch stopwatch = Stopwatch.StartNew();

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
                        Log.Message($"Processed {i + 1}/{tokens.Count} tokens in {tokenStopwatch.ElapsedMilliseconds} ms.");
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

    public async Task BuildTokenTable(string inputFile)
    {
        var tables = _tableManager.GetTables();
        if (!tables.ContainsKey(TableNames.Tokens))
        {
            _tableManager.CreateTable(TableNames.Tokens);
        }

        _embeddingStorage.ResetEmbeddings();

        Stopwatch stopwatch = Stopwatch.StartNew();

        var semaphore = new SemaphoreSlim(1);

        await semaphore.WaitAsync(_cts.Token);

        try
        {
            await Task.Run(async () =>
            {
                try
                {
                    Log.Message($"Starting to build token table from '{inputFile}'");
                    var tokenStopwatch = Stopwatch.StartNew();

                    if (!File.Exists(inputFile))
                    {
                        throw new FileNotFoundException($"File not found: {inputFile}");
                    }

                    Log.Message($"Loading file: {inputFile}");
                    string jsonData = await File.ReadAllTextAsync(inputFile);

                    Log.Message($"File loaded successfully. Parsing JSON data...");
                    TokenList tokenList = JsonUtility.FromJson<TokenList>(jsonData);
                    Log.Message($"JSON data parsed successfully. Total tokens: {tokenList.tokens.Count}");

                    await TokenOptimizerTask.Execute(inputFile);

                    jsonData = await File.ReadAllTextAsync(inputFile);
                    tokenList = JsonUtility.FromJson<TokenList>(jsonData);

                    Log.Message($"Adding {tokenList.tokens.Count} embeddings to '{TableNames.Tokens}'...");

                    foreach (var token in tokenList.tokens)
                    {
                        try
                        {
                            if (!_embeddingStorage.DoesTokenExist(token.Name))
                            {
                                _embeddingStorage.Add(token.Name, token.Vector.ToArray(), TableNames.Tokens, EmbeddingType.Token);
                            }
                            else
                            {
                                Log.Message($"Duplicate token '{token.Name}' found, skipping.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"Error processing token '{token.Name}': {ex.Message}");
                            throw;
                        }
                    }

                    tokenStopwatch.Stop();
                    Log.Message($"Token table built successfully in {tokenStopwatch.ElapsedMilliseconds} ms.");
                }
                catch (OperationCanceledException)
                {
                    Log.Error("Operation canceled.");
                    throw;
                }
                catch (Exception ex)
                {
                    Log.Error($"Unexpected error: {ex.Message}");
                    throw;
                }
                finally
                {
                    semaphore.Release();
                }
            }, _cts.Token);
        }
        catch (Exception ex)
        {
            Log.Error($"Unhandled exception: {ex.Message}");
            throw;
        }

        stopwatch.Stop();
        double totalTimeMinutes = stopwatch.Elapsed.TotalMinutes;
        Log.Message($"Total time: {totalTimeMinutes:F2} minutes");
        Log.Message($"Table '{TableNames.Tokens}' built successfully.");
    }
}
