using CommandTerminal;
using System;

public static class TableListAction
{
    public static void Execute(CommandArg[] args)
    {
        try
        {
            Database.Instance.ListTables();
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while executing the list tables command: {ex.Message}");
        }
    }
}
