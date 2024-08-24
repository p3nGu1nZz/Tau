using CommandTerminal;
using System;

public static class TableDeleteAction
{
    public static void Execute(CommandArg[] args)
    {
        try
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("Insufficient arguments for delete table command.");
            }

            string tableName = args[1].String;
            Database.Instance.DeleteTable(tableName);
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while executing the delete table command: {ex.Message}");
        }
    }
}
