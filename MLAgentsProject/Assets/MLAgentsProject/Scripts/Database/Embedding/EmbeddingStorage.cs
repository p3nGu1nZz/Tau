using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;

public class EmbeddingStorage
{
    private DatabaseInfo _info;
    private DatabaseManager _tableManager;
    private ConcurrentDictionary<string, bool> _tokens;

    public EmbeddingStorage(DatabaseInfo info, DatabaseManager tableManager)
    {
        _info = info;
        _tableManager = tableManager;
        _tokens = new ConcurrentDictionary<string, bool>();
        Log.Message("EmbeddingStorage initialized.");
    }

    public void Add(string token, double[] embedding, string tableName, EmbeddingType type)
    {
        EmbeddingUtilities.ValidateEmbeddingSize(embedding);
        EmbeddingUtilities.ValidateTableName(tableName);

        var tables = _tableManager.GetTables();
        if (!tables.ContainsKey(tableName))
        {
            throw new ArgumentException($"Table '{tableName}' does not exist.");
        }

        var newEmbedding = new Embedding(Database.Instance.GenerateUniqueId(), token, embedding, type);
        tables[tableName].Add(newEmbedding);
        _tokens[token] = true;

        // Update database info
        _info.TotalEmbeddings++;
        _info.DatabaseSize += embedding.Length * sizeof(double);

        Log.Message($"Added embedding for token '{token}' to table '{tableName}'.");
    }

    public async Task<double[]> GetEmbedding(string token, EmbeddingType type)
    {
        Log.Message($"Starting to retrieve embedding for token: '{token}'");

        var tables = _tableManager.GetTables();
        foreach (var table in tables.Values)
        {
            var embedding = table.FirstOrDefault(e => e.Token == token)?.Vector;
            if (embedding != null)
            {
                Log.Message($"Found embedding for token '{token}' in existing tables.");
                return embedding.ToArray();
            }
        }

        Log.Message($"Generating new embedding for token '{token}'.");
        double[] newEmbedding = await GenerateEmbeddingTask.Execute(token, type);

        if (newEmbedding != null)
        {
            Log.Message($"Successfully generated new embedding for token '{token}'.");
        }
        else
        {
            Log.Error($"Failed to generate new embedding for token '{token}'.");
        }

        return newEmbedding;
    }

    public bool DoesTokenExist(string token)
    {
        return _tokens.ContainsKey(token);
    }

    public void ResetEmbeddings()
    {
        _tokens.Clear();
        Log.Message("Tokens have been reset.");
    }

    public Embedding Match(double[] vector)
    {
        Log.Message("Starting Match function...");
        EmbeddingUtilities.ValidateEmbeddingSize(vector);

        var table = _tableManager.GetTable(TableNames.Vocabulary);
        if (table == null)
        {
            Log.Error($"Table '{TableNames.Vocabulary}' does not exist.");
            throw new ArgumentException($"Table '{TableNames.Vocabulary}' does not exist.");
        }

        List<Embedding> embeddings = table.Values.ToList();
        Log.Message($"Retrieved {embeddings.Count} embeddings from table '{TableNames.Vocabulary}'.");

        NativeArray<double> vectors = new NativeArray<double>(embeddings.SelectMany(e => e.Vector).ToArray(), Allocator.TempJob);
        NativeArray<double> query = new NativeArray<double>(vector, Allocator.TempJob);
        NativeArray<double> results = new NativeArray<double>(embeddings.Count, Allocator.TempJob);

        Log.Message("Starting CosineSimilarityJob...");
        CosineSimilarityJob job = new CosineSimilarityJob
        {
            Vectors = vectors,
            QueryVector = query,
            Results = results,
            VectorSize = DatabaseConstants.VectorSize
        };

        JobHandle handle = job.Schedule(embeddings.Count, 64);
        handle.Complete();
        Log.Message("CosineSimilarityJob completed.");

        int closestIndex = 0;
        double maxSimilarity = results[0];
        for (int i = 1; i < results.Length; i++)
        {
            if (results[i] > maxSimilarity)
            {
                maxSimilarity = results[i];
                closestIndex = i;
            }
        }

        vectors.Dispose();
        query.Dispose();
        results.Dispose();

        Log.Message($"Retrieved closest embedding for query vector from table '{TableNames.Vocabulary}' with similarity {maxSimilarity:F4}.");

        return embeddings[closestIndex];
    }
}
