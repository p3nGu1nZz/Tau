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

        if (command == CommandNames.Table && args.Length >= 2)
        {
            string _command = args[1].String.ToLower();
            if (DatabaseTableAction.Actions.TryGetValue(_command, out var _action))
            {
                return _action;
            }
        }
        else
        {
            throw new ArgumentOutOfRangeException();
        }

        return null;
    }
}
