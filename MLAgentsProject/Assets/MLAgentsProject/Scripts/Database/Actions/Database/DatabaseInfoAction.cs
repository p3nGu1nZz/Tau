using CommandTerminal;
using System;

public static class DatabaseInfoAction
{
    public static void Execute(CommandArg[] args)
    {
        try
        {
            Log.Message(Database.Instance.GetInfo());
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while executing the info command: {ex.Message}");
        }
    }
}
