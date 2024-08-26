using CommandTerminal;
using System;

public static class DatabaseSaveAction
{
    public static void Execute(CommandArg[] args)
    {
        try
        {
            string fileName = args.Length >= 2 ? args[1].String : null;
            Database.Instance.Save(fileName);
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while executing the save command: {ex.Message}");
        }
    }
}
