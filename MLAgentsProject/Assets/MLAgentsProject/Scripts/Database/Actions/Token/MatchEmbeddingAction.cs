using CommandTerminal;
using System;
using UnityEngine;

public static class MatchEmbeddingAction
{
    public static void Execute(CommandArg[] args)
    {
        if (args.Length < 1)
        {
            throw new ArgumentException($"Insufficient arguments. Usage: token match embedding <{DatabaseConstants.VectorSize} float values>");
        }

        if (!Database.Instance.IsLoaded())
        {
            throw new InvalidOperationException("Database is not loaded. Please load the database before executing this command.");
        }

        string embeddingString = args[0].String;
        double[] embedding = StringUtilities.ConvertStringToVector(embeddingString);

        var token = Database.Instance.Match(embedding);
        if (token != null)
        {
            Debug.Log($"Token for embedding '{StringUtilities.ConvertVectorToString(embedding)}': {token}");
        }
        else
        {
            Debug.Log($"Embedding not found in the vocabulary.");
        }
    }
}
