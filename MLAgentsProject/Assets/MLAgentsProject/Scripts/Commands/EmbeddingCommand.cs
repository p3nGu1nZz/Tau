using CommandTerminal;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EmbeddingCommand : BaseCommand<EmbeddingCommand>
{
    protected override Dictionary<string, Action<CommandArg[]>> CommandActions { get; } = new()
    {
        { "match", MatchCommand }
    };

    [RegisterCommand(Help = "Manages embedding operations", MinArgCount = 1)]
    public static void CommandEmbedding(CommandArg[] args)
    {
        Execute(args);
    }

    private static void MatchCommand(CommandArg[] args)
    {
        if (args.Length < DatabaseConstants.VectorSize)
        {
            Log.Error("Insufficient arguments. Usage: embedding match <384 float values>");
            return;
        }

        double[] embedding = args.Select(arg => (double)arg.Float).ToArray();
        var token = Database.Instance.Match(embedding);
        if (token != null)
        {
            Debug.Log($"Token for embedding '{string.Join(", ", embedding)}': {token}");
        }
        else
        {
            Debug.Log($"Embedding not found in the vocabulary.");
        }
    }
}
