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
                "  check <filename>      Checks the integrity of the specified data file",
                "  concat <filenames>    Concatenates multiple data files into one, or processes all JSON files in a directory",
                "  delete <filename>     Deletes the specified data file",
                "  help                  Displays this help message",
                "  info <filename>       Displays metadata and data counts for the specified file",
                "  list                  Lists all JSON data files in the Data directory",
                "  load <filename>       Loads training data from one or more JSON files",
                "  ophrase <filename>    Processes data with ophrase to generate paraphrased responses",
                "  prune <filename>      Prunes the specified data file by removing extra punctuation spaces and must exists in database",
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
