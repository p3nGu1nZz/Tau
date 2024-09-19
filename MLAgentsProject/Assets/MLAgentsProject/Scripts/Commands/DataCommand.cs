using CommandTerminal;
using System;
using System.Collections.Generic;
using System.Linq;

public static class DataCommand
{
    private static readonly Dictionary<string, Action<CommandArg[]>> CommandActions = new()
    {
        { "load", DataLoadAction.Execute },
        { "check", DataCheckAction.Execute },
        { "prune", DataPruneAction.Execute },
        { "concat", DataConcatAction.Execute },
        { "ophrase", DataOphraseAction.Execute },
        { "oproof", DataOproofAction.Execute },
        { "remove", DataRemoveAction.Execute },
        { "info", DataInfoAction.Execute },
        { "list", DataListAction.Execute },
        { "help", DataHelpAction.Execute }
    };

    [RegisterCommand(Help = "Loads training data from one or more JSON files in the Scripts directory", MinArgCount = 1)]
    public static void CommandData(CommandArg[] args)
    {
        try
        {
            if (args.Length < 1)
            {
                throw new ArgumentException("Insufficient arguments.");
            }

            string command = args[0].String.ToLower();
            var commandAction = GetCommandAction(command, args);
            if (commandAction != null)
            {
                commandAction(args.Skip(1).ToArray());
            }
            else
            {
                throw new ArgumentException("Invalid command or insufficient arguments.");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while executing the command: {ex.Message}");
        }
    }

    private static Action<CommandArg[]> GetCommandAction(string command, CommandArg[] args)
    {
        if (CommandActions.TryGetValue(command, out var commandAction))
        {
            return commandAction;
        }

        return null;
    }
}
