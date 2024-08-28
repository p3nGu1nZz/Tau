using CommandTerminal;
using System;

public static class TableCreateAction
{
    public static void Execute(CommandArg[] args)
    {
        try
        {
            if (args.Length < 3)
            {
                throw new ArgumentException("Insufficient arguments for create table command. Usage: database table create <table_name>");
            }

            string tableName = args[2].String;
            Database.Instance.CreateTable(tableName);
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while executing the create table command: {ex.Message}");
        }
    }
}
