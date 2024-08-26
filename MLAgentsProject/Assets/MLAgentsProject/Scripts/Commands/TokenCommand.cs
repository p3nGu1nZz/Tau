using CommandTerminal;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TokenCommand : BaseCommand<TokenCommand>
{
    protected override Dictionary<string, Action<CommandArg[]>> CommandActions { get; } = new()
    {
        { "match", MatchCommand }
    };

    private static readonly Dictionary<string, Action<CommandArg[]>> MatchCommandActions = new()
    {
        { "embedding", MatchEmbeddingAction.Execute }
    };

    [RegisterCommand(Help = "Manages token operations", MinArgCount = 1)]
    public static void CommandToken(CommandArg[] args)
    {
        try
        {
            Execute(args);
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while executing the command: {ex.Message}");
            throw;
        }
    }

    protected override Action<CommandArg[]> GetCommandAction(string command, CommandArg[] args)
    {
        if (CommandActions.TryGetValue(command, out var commandAction))
        {
            return commandAction;
        }

        if (command == "match" && args.Length >= 2)
        {
            string matchCommand = args[1].String.ToLower();
            if (MatchCommandActions.TryGetValue(matchCommand, out var matchCommandAction))
            {
                return matchCommandAction;
            }
        }

        return null;
    }

    private static void MatchCommand(CommandArg[] args)
    {
        if (args.Length < 2)
        {
            throw new ArgumentException($"Insufficient arguments. Usage: token match embedding <{DatabaseConstants.VectorSize} float values>");
        }

        string subCommand = args[0].String.ToLower();
        if (MatchCommandActions.TryGetValue(subCommand, out var subCommandAction))
        {
            subCommandAction(args.Skip(1).ToArray());
        }
        else
        {
            throw new ArgumentException($"Invalid subcommand. Usage: token match embedding <{DatabaseConstants.VectorSize} float values>");
        }
    }
}
