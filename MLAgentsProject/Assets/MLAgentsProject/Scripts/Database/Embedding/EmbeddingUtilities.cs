using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public static class EmbeddingUtilities
{
    public const int MaxConcurrentJobs = 8;

    public static void ValidateEmbeddingSize(double[] embedding, int expectedSize = 384)
    {
        if (embedding.Length != expectedSize)
        {
            throw new ArgumentException($"Embedding vector must be of size {expectedSize}.");
        }
    }

    public static void ValidateTableName(string tableName)
    {
        if (string.IsNullOrEmpty(tableName))
        {
            throw new ArgumentException("Table name must be provided.");
        }
    }

    public static void WriteTokensToFile(List<string> tokens, string filePath)
    {
        File.WriteAllLines(filePath, tokens);
        Log.Message($"Tokens exported successfully to {filePath}");
    }

    public static async Task AggregateAndStoreEmbeddings(Dictionary<string, List<string>> chunkedTexts, string tableName)
    {
        Log.Message($"Starting aggregation and storage of embeddings for table: {tableName}");

        foreach (var kvp in chunkedTexts)
        {
            string originalText = kvp.Key;
            List<string> chunks = kvp.Value;

            Log.Message($"Processing original text: {originalText} with {chunks.Count} chunks");

            List<double[]> embeddings = new List<double[]>();

            foreach (var chunk in chunks)
            {
                EmbeddingType chunkType = DetermineEmbeddingType(chunk, true);
                double[] embedding = await Database.Instance.GetEmbedding(chunk, chunkType);
                if (embedding != null)
                {
                    embeddings.Add(embedding);
                    Log.Message($"Successfully retrieved embedding for chunk: {chunk}");
                }
                else
                {
                    Log.Error($"Failed to get embedding for chunk: {chunk}");
                }
            }

            if (embeddings.Count > 0)
            {
                double[] averagedEmbedding = embeddings.Aggregate((acc, emb) => acc.Zip(emb, (a, b) => a + b).ToArray()).Select(x => x / embeddings.Count).ToArray();
                EmbeddingType originalTextType = DetermineEmbeddingType(originalText);
                Database.Instance.Add(originalText, averagedEmbedding, tableName, originalTextType);
                Log.Message($"Averaged embedding stored for original text: {originalText}");
            }
            else
            {
                Log.Warning($"No embeddings found for original text: {originalText}");
            }
        }

        Log.Message($"Completed aggregation and storage of embeddings for table: {tableName}");
    }

    public static EmbeddingType DetermineEmbeddingType(string token, bool isChunk = false)
    {
        // Preprocess the token
        string trimmedToken = token.Trim();
        string[] words = trimmedToken.Split(' ');

        // Determine the embedding type using a switch case
        return isChunk switch
        {
            true => EmbeddingType.Chunk,
            false => words.Length switch
            {
                <= 128 => EmbeddingType.Short,
                _ => EmbeddingType.Long,
            }
        };
    }

    public static bool IsValidVectorFormat(string vectorString)
    {
        // Regex to match the vector format "[.double, .double, ...]"
        string pattern = @"^\[\s*(-?\d+(\.\d{1,21})?\s*,\s*)*(-?\d+(\.\d{1,21})?\s*)\]$";
        return Regex.IsMatch(vectorString, pattern);
    }
}
