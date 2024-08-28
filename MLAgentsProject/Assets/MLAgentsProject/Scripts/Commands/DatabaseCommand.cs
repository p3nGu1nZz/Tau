using CommandTerminal;
using System;
using System.Collections.Generic;

public class DatabaseCommand : BaseCommand<DatabaseCommand>
{
    protected override Dictionary<string, Action<CommandArg[]>> Actions { get; } = new()
    {
        { "load", DatabaseLoadAction.Execute },
        { "save", DatabaseSaveAction.Execute },
        { "info", DatabaseInfoAction.Execute },
        { "find", DatabaseFindAction.Execute },
        { "table", DatabaseTableAction.Execute },
        { "help", DatabaseHelpAction.Execute }
    };

    [RegisterCommand(Help = "Manages the Vector Database", MinArgCount = 1)]
    public static void CommandDatabase(CommandArg[] args)
    {
        Execute(args);
    }

    protected override Action<CommandArg[]> GetAction(string command, CommandArg[] args)
    {
        if (Actions.TryGetValue(command, out var action))
        {
            return action;
        }

        if (command == "table" && args.Length >= 2)
        {
            string subCommand = args[1].String.ToLower();
            if (DatabaseTableAction.Actions.TryGetValue(subCommand, out var subAction))
            {
                return subAction;
            }
        }
        else
        {
            throw new ArgumentOutOfRangeException();
        }

        return null;
    }
}
