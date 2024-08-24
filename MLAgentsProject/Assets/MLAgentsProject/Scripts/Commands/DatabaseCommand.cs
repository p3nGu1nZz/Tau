using CommandTerminal;
using System;
using System.Collections.Generic;
using System.Linq;

public static class DatabaseCommand
{
    private static readonly Dictionary<string, Action<CommandArg[]>> CommandActions = new()
    {
        { "load", DatabaseLoadAction.Execute },
        { "save", DatabaseSaveAction.Execute },
        { "info", DatabaseInfoAction.Execute },
        { "table", DatabaseTableAction.Execute },
        { "find", DatabaseFindAction.Execute }
    };

    [RegisterCommand(Help = "Manages the Vector Database", MinArgCount = 1)]
    public static void CommandDatabase(CommandArg[] args)
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
                commandAction(args);
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

        if (command == "table" && args.Length >= 2)
        {
            string tableCommand = args[1].String.ToLower();
            if (DatabaseTableAction.TableCommandActions.TryGetValue(tableCommand, out var tableCommandAction))
            {
                return tableCommandAction;
            }
        }

        return null;
    }
}
