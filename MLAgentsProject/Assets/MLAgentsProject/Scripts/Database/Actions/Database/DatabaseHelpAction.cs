using CommandTerminal;
using System;
using System.Collections.Generic;

public static class DatabaseHelpAction
{
    public static void Execute(CommandArg[] args)
    {
        try
        {
            var helpText = new List<string>
            {
                "Usage: database <subcommand> [options]",
                "",
                "Subcommands:",
                "  load <filename>       Loads the database from the specified file",
                "  save <filename>       Saves the database to the specified file",
                "  info                  Displays information about the database",
                "  find <token>          Finds a token in the database",
                "  table <subcommand>    Manages database tables",
                "  help                  Displays this help message",
                "",
                "Table Subcommands:",
                "  create <table_name>   Creates a new table",
                "  remove <table_name>   Removes the specified table",
                "  list                  Lists all tables in the database",
                "  view <table_name>     Displays the contents of the specified table",
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
