using CommandTerminal;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TokenCommand : BaseCommand<TokenCommand>
{
    protected override Dictionary<string, Action<CommandArg[]>> CommandActions { get; } = new()
    {
        { "match", MatchCommand }
    };

    [RegisterCommand(Help = "Manages token operations", MinArgCount = 1)]
    public static void CommandToken(CommandArg[] args)
    {
        Execute(args);
    }

    private static void MatchCommand(CommandArg[] args)
    {
        if (args.Length < 1)
        {
            Log.Error("Insufficient arguments. Usage: token match <token>");
            return;
        }

        string token = args[0].String;
        var embedding = Database.Instance.FindInTables(token);
        if (embedding != null)
        {
            Debug.Log($"Embedding for token '{token}': {string.Join(", ", embedding)}");
        }
        else
        {
            Debug.Log($"Token '{token}' not found in the vocabulary.");
        }
    }
}
