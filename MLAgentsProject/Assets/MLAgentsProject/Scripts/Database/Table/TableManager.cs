using System.Collections.Generic;
using System.Linq;

public class TableManager
{
    private readonly Dictionary<string, List<Embedding>> _tables;
    private readonly DatabaseInfo _info;

    public TableManager(Dictionary<string, List<Embedding>> tables, DatabaseInfo info)
    {
        _tables = tables;
        _info = info;
    }

    public void CreateTable(string tableName)
    {
        if (_tables.TryAdd(tableName, new List<Embedding>()))
        {
            _info.TableCount++;
            Log.Message($"Table '{tableName}' created successfully.");
        }
        else
        {
            Log.Error($"Table '{tableName}' already exists.");
        }
    }

    public void DeleteTable(string tableName)
    {
        if (_tables.Remove(tableName))
        {
            _info.TableCount--;
            Log.Message($"Table '{tableName}' deleted successfully.");
        }
        else
        {
            Log.Error($"Table '{tableName}' does not exist.");
        }
    }

    public void ListTables()
    {
        Log.Message("Listing tables:");
        foreach (var table in _tables.Keys)
        {
            Log.Message($"- {table}");
        }
    }

    public void Clear()
    {
        _tables.Clear();
        _info.TotalEmbeddings = 0;
        _info.DatabaseSize = 0;
        _info.TableCount = 0;
        Log.Message("All tables cleared.");
    }

    public Dictionary<string, List<Embedding>> GetTables()
    {
        Log.Message("Retrieving all tables.");
        return _tables;
    }

    public Dictionary<string, Embedding> GetTable(string tableName)
    {
        if (_tables.TryGetValue(tableName, out var table))
        {
            return table.ToDictionary(e => e.Token, e => e);
        }
        else
        {
            Log.Error($"Table '{tableName}' does not exist.");
            return null;
        }
    }

    public bool HasTable(string tableName)
    {
        return _tables.ContainsKey(tableName);
    }

    public bool IsLoaded()
    {
        bool isLoaded = _info.TableCount >= 3 &&
                        _info.TotalEmbeddings > 0 &&
                        HasTable("vocabulary") &&
                        HasTable("training_data") &&
                        HasTable("evaluation_data");

        Log.Message($"IsLoaded check: {isLoaded}");

        return isLoaded;
    }

    public double[] FindEmbedding(string tableName, string token)
    {
        if (_tables.TryGetValue(tableName, out var table))
        {
            var embedding = table.FirstOrDefault(e => e.Token == token)?.Vector;
            if (embedding != null)
            {
                return embedding.ToArray();
            }
        }
        return null;
    }
}
