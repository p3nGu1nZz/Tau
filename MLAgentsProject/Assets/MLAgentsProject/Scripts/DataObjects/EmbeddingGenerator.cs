using System;
using System.Threading.Tasks;

public class EmbeddingGenerator
{
    public static async Task<double[]> GenerateEmbedding(string token, EmbeddingType type)
    {
        string result = await Encoder.Instance.Encode(token);

        if (string.IsNullOrEmpty(result))
        {
            Log.Error($"Failed to encode token '{token}'.");
            return null;
        }

        Log.Message($"Encoder Result for '{token}': {result[..Math.Min(result.Length, 90)]}...");

        Embedding embedding = Embedding.FromEncoder(0, token, result, type);

        if (embedding == null)
        {
            Log.Error($"Failed to create embedding from encoder result for token '{token}'.");
            return null;
        }

        return embedding.Vector.ToArray();
    }
}
