using CommandTerminal;
using System;
using System.Collections.Generic;
using System.Linq;

public static class DatabaseTableAction
{
    public static void Execute(CommandArg[] args)
    {
        try
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("Insufficient arguments for table command.");
            }

            string tableCommand = args[1].String.ToLower();
            if (TableCommandActions.TryGetValue(tableCommand, out var tableCommandAction))
            {
                tableCommandAction(args.Skip(1).ToArray());
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

    public static readonly Dictionary<string, Action<CommandArg[]>> TableCommandActions = new()
    {
        { "create", TableCreateAction.Execute },
        { "delete", TableDeleteAction.Execute },
        { "list", TableListAction.Execute },
        { "view", TableViewAction.Execute }
    };
}
