using CommandTerminal;
using System;
using System.Collections.Generic;

public static class TrainingHelpAction
{
    public static void Execute(CommandArg[] args)
    {
        try
        {
            var helpText = new List<string>
            {
                "Usage: training <subcommand> [options]",
                "",
                "Subcommands:",
                "  agent <type> <filename> [--agents <number>]  Trains a specified agent type with the given filename and optional number of agents",
                "  cancel                                           Cancels ongoing training",
                "  help                                             Displays this help message",
                ""
            };

            foreach (var line in helpText)
            {
                Log.Message(line);
            }
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while displaying the help message: {ex.Message}");
        }
    }
}
