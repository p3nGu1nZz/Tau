using CommandTerminal;
using System;

public static class TableViewAction
{
    public static void Execute(CommandArg[] args)
    {
        try
        {
            if (args.Length < 3)
            {
                throw new ArgumentException("Insufficient arguments for view table command. Usage: database table view <table_name>");
            }

            string tableName = args[2].String;
            Database.Instance.ViewTable(tableName);
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while executing the view table command: {ex.Message}");
        }
    }
}
