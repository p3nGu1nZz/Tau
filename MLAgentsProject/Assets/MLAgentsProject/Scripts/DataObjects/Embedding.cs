using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class Embedding
{
    public int Id;
    public string Token;
    public List<double> Vector;
    public EmbeddingType Type;

    public Embedding(int id, string token, double[] vector, EmbeddingType type)
    {
        if (vector.Length != Constants.VectorSize)
        {
            throw new ArgumentException("Embedding vector must be of size 384.");
        }

        Id = id;
        Token = token;
        Vector = new List<double>(vector);
        Type = type;
    }

    public static Embedding FromEncoder(int id, string token, string encoderResult, EmbeddingType type)
    {
        EncoderResult embeddingResult;
        try
        {
            embeddingResult = JsonUtility.FromJson<EncoderResult>(encoderResult);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to deserialize encoder result: {ex.Message}");
            return null;
        }

        if (embeddingResult.Embeddings == null)
        {
            Debug.LogError("Embeddings field is null in the deserialized result.");
            return null;
        }

        return new Embedding(id, token, embeddingResult.Embeddings, type);
    }
}
