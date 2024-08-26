using CommandTerminal;
using System;

public static class DatabaseLoadAction
{
    public static void Execute(CommandArg[] args)
    {
        try
        {
            string fileName = args.Length >= 2 ? args[1].String : null;
            Database.Instance.Load(fileName);
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while executing the load command: {ex.Message}");
        }
    }
}
