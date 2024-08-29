using CommandTerminal;
using System;
using System.Linq;

public static class DatabaseFindAction
{
    public static void Execute(CommandArg[] args)
    {
        try
        {
            Log.Message("Starting DatabaseFindAction...");

            if (args.Length < 2)
            {
                throw new ArgumentException("Insufficient arguments for find command. Usage: find \"<token>\" [\"<table_name>\"]");
            }

            string joinedArgs = string.Join(" ", args.Skip(1).Select(arg => arg.String));
            Log.Message($"Joined arguments: '{joinedArgs}'");

            int firstQuoteIndex = joinedArgs.IndexOf('"');
            if (firstQuoteIndex == -1)
            {
                throw new ArgumentException("Token must be enclosed in double or single quotes.");
            }

            string tableName = firstQuoteIndex > 0 ? joinedArgs.Substring(0, firstQuoteIndex).Trim() : null;
            string remainingArgs = joinedArgs.Substring(firstQuoteIndex).Trim();

            string token = StringUtilities.ExtractQuotedString(ref remainingArgs);

            Log.Message($"Extracted token: '{token}'");
            Log.Message($"Extracted tableName: '{tableName}'");

            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Token must be enclosed in double or single quotes.");
            }

            Log.Message($"Searching database for '{token}' in table '{tableName}'");
            Database.Instance.FindInTables(token, tableName);
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while executing the command: {ex.Message}");
        }
    }
}
