using CommandTerminal;
using System;
using System.Collections.Generic;

public static class DatabaseTableAction
{
    public static void Execute(CommandArg[] args)
    {
        try
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("Insufficient arguments for table command. Usage: database table <subcommand> <table_name> <args>");
            }

            string subCommand = args[1].String.ToLower();
            if (Actions.TryGetValue(subCommand, out var action))
            {
                action(args);
            }
            else
            {
                throw new ArgumentException("Invalid table command.");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while executing the table command: {ex.Message}");
        }
    }

    public static readonly Dictionary<string, Action<CommandArg[]>> Actions = new()
    {
        { "build", TableBuildAction.Execute },
        { "create", TableCreateAction.Execute },
        { "remove", TableRemoveAction.Execute },
        { "list", TableListAction.Execute },
        { "view", TableViewAction.Execute }
    };
}
