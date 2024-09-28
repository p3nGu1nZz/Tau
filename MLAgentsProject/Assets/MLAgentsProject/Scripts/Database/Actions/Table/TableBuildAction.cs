using CommandTerminal;
using System;
using System.Collections.Generic;

public static class TableBuildAction
{
    public static async void Execute(CommandArg[] args)
    {
        try
        {
            if (args.Length < 3)
            {
                throw new ArgumentException("Invalid arguments for build tokens command. Usage: database table build tokens <tableName>");
            }

            string tableName = args[2].String.ToLower();
            if (tableName != TableNames.Tokens)
            {
                throw new ArgumentException($"Invalid table name. Only '{TableNames.Tokens}' table can be built using this command.");
            }

            Log.Message($"Building '{tableName}' table...");

            List<string> vocabulary = Database.Instance.GetVocabulary();
            string tokenFileName = await Tokenizer.Export(vocabulary);
            await Database.Instance.BuildTokenTable(tokenFileName);

            Log.Message($"'{tableName}' table built successfully.");
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while executing the build tokens command: {ex.Message}");
        }
    }
}
