using CommandTerminal;
using System;
using System.Linq;
using UnityEngine;

public static class MatchTokenAction
{
    public static void Execute(CommandArg[] args)
    {
        if (args.Length < 1)
        {
            throw new ArgumentException("Insufficient arguments. Usage: embedding match token <token>");
        }

        if (!Database.Instance.IsLoaded())
        {
            throw new InvalidOperationException("Database is not loaded. Please load the database before executing this command.");
        }

        string token = args[0].String;
        var results = Database.Instance.FindInTables(token);
        if (results != null && results.Count > 0)
        {
            var embedding = results.First().Embedding.Vector.ToArray();
            Debug.Log($"Embedding for token '{token}': {StringUtilities.ConvertVectorToString(embedding)}");
        }
        else
        {
            Debug.Log($"Token '{token}' not found in the vocabulary.");
        }
    }
}
