using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public static class DatabaseUtilities
{
    public static void SaveToFile(string content, string filePath)
    {
        File.WriteAllText(filePath, content);
        Debug.Log($"File saved successfully to {filePath}");
    }

    public static void CompressStringToFile(string content, string compressedFileName)
    {
        using (var memoryStream = new MemoryStream())
        using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(content);
            gzipStream.Write(bytes, 0, bytes.Length);
            gzipStream.Close();
            File.WriteAllBytes(compressedFileName, memoryStream.ToArray());
            Debug.Log($"Compressed string to {compressedFileName}");
        }
    }

    public static string ExtractToString(string compressedFileName)
    {
        using (var compressedFileStream = new FileStream(compressedFileName, FileMode.Open))
        using (var decompressedStream = new MemoryStream())
        using (var gzipStream = new GZipStream(compressedFileStream, CompressionMode.Decompress))
        {
            gzipStream.CopyTo(decompressedStream);
            return System.Text.Encoding.UTF8.GetString(decompressedStream.ToArray());
        }
    }

    public static async Task BuildAndPopulateTables(List<string> vocabulary, List<string> combinedTrainingData, List<string> combinedEvaluationData)
    {
        Database.Instance.Clear();
        Log.Message("Vector database ready.");

        await Database.Instance.BuildTable(vocabulary, TableNames.Vocabulary, true);
        Log.Message("Vector database populated successfully with vocabulary.");

        await Database.Instance.BuildTable(combinedTrainingData, TableNames.TrainingData);
        Log.Message("Vector database populated successfully with combined training data.");

        await Database.Instance.BuildTable(combinedEvaluationData, TableNames.EvaluationData);
        Log.Message("Vector database populated successfully with combined evaluation data.");
    }

    public static async Task AggregateAndStoreEmbeddings(Dictionary<string, List<string>> chunkedTrainingData, Dictionary<string, List<string>> chunkedEvaluationData)
    {
        await EmbeddingUtilities.AggregateAndStoreEmbeddings(chunkedTrainingData, "training_data");
        await EmbeddingUtilities.AggregateAndStoreEmbeddings(chunkedEvaluationData, "evaluation_data");
    }
}
