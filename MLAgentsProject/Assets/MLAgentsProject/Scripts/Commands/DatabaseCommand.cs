using CommandTerminal;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseCommand : BaseCommand<DatabaseCommand>
{
    protected override Dictionary<string, Action<CommandArg[]>> CommandActions { get; } = new()
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
        Execute(args);
    }

    protected override Action<CommandArg[]> GetCommandAction(string command, CommandArg[] args)
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
