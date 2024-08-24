using System;
using System.Collections.Generic;
using System.Linq;

public class TableSearcher
{
    private readonly Dictionary<string, List<Embedding>> _tables;

    public TableSearcher(Dictionary<string, List<Embedding>> tables)
    {
        _tables = tables;
    }

    public List<(string TableName, Embedding Embedding)> SearchInTables(string token, bool searchAllTables, string tableName = null)
    {
        var results = new List<(string TableName, Embedding Embedding)>();

        if (searchAllTables)
        {
            foreach (var table in _tables.Keys)
            {
                var tableResults = SearchInTable(table, token);
                results.AddRange(tableResults);
            }
        }
        else
        {
            var tableResults = SearchInTable(tableName, token);
            results.AddRange(tableResults);
        }

        return results;
    }

    public List<(string TableName, Embedding Embedding)> SearchInTable(string tableName, string token)
    {
        Log.Message($"Searching for token '{token}' in table '{tableName}'.");

        var table = GetTable(tableName);
        if (table == null)
        {
            throw new ArgumentException($"Table '{tableName}' does not exist.");
        }

        Log.Message($"Table '{tableName}' contains {table.Count} entries.");

        var results = new List<(string TableName, Embedding Embedding)>();

        if (table.TryGetValue(token, out var embedding))
        {
            Log.Message($"Token '{token}' found in table '{tableName}'.");
            results.Add((tableName, embedding));
        }
        else
        {
            Log.Message($"Token '{token}' not found in table '{tableName}'.");
        }

        return results;
    }

    private Dictionary<string, Embedding> GetTable(string tableName)
    {
        if (_tables.TryGetValue(tableName, out var table))
        {
            Log.Message($"Retrieved table '{tableName}' with {table.Count} entries.");
            return table.ToDictionary(e => e.Token, e => e);
        }
        else
        {
            Log.Message($"Table '{tableName}' does not exist.");
            return null;
        }
    }
}
