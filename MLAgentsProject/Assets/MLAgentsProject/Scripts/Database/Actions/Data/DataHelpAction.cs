using CommandTerminal;
using System;
using System.Collections.Generic;

public static class DataHelpAction
{
    public static void Execute(CommandArg[] args)
    {
        try
        {
            var helpText = new List<string>
            {
                "Usage: data <subcommand> [options]",
                "",
                "Subcommands:",
                "  load <filename>       Loads training data from one or more JSON files",
                "  check <filename>      Checks the integrity of the specified data file",
                "  concat <filenames>    Concatenates multiple data files into one",
                "  info <filename>       Displays metadata and data counts for the specified file",
                "  delete <filename>     Deletes the specified data file",
                "  list                  Lists all JSON data files in the Data directory",
                "  help                  Displays this help message",
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
            throw;
        }
    }
}
