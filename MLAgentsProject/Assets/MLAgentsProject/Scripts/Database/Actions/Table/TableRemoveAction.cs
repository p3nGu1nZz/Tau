using CommandTerminal;
using System;

public static class TableRemoveAction
{
    public static void Execute(CommandArg[] args)
    {
        try
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("Insufficient arguments for remove table command.");
            }

            string tableName = args[2].String;
            Database.Instance.RemoveTable(tableName);
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while executing the remove table command: {ex.Message}");
        }
    }
}
