using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class DatabaseManager
{
    private readonly Dictionary<string, List<Embedding>> _tables = new();
    private readonly DatabaseInfo _info;
    private readonly TableManager _tableManager;
    private readonly SaveLoadManager _saveLoadManager;
    private readonly SearchDisplayManager _searchDisplayManager;

    public DatabaseManager(DatabaseInfo info)
    {
        _info = info;
        _tableManager = new TableManager(_tables, _info);
        _saveLoadManager = new SaveLoadManager(_tables, _info);
        _searchDisplayManager = new SearchDisplayManager(_tables);
        Log.Message("DatabaseManager initialized.");
    }

    public void CreateTable(string tableName) => _tableManager.CreateTable(tableName);
    public void DeleteTable(string tableName) => _tableManager.DeleteTable(tableName);
    public void ListTables() => _tableManager.ListTables();
    public void Save(string fileName = DatabaseConstants.DatabaseFileName) => _saveLoadManager.Save(fileName);
    public void Load(string fileName = DatabaseConstants.DatabaseFileName) => _saveLoadManager.Load(fileName);
    public void Clear() => _tableManager.Clear();
    public Dictionary<string, List<Embedding>> GetTables() => _tableManager.GetTables();
    public Dictionary<string, Embedding> GetTable(string tableName) => _tableManager.GetTable(tableName);
    public void ViewTable(string tableName) => _searchDisplayManager.ViewTable(tableName);
    public bool IsLoaded() => _tableManager.IsLoaded();
}
