using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Database
{
    private static Database _instance;
    public static Database Instance => _instance ??= new Database();

    public DatabaseInfo Info { get; private set; }
    private DatabaseManager _databaseManager;
    private EmbeddingManager _embeddingManager;
    private int _currentId = 1;

    private Database()
    {
        Info = new DatabaseInfo();
        _databaseManager = new DatabaseManager(Info);
        _embeddingManager = new EmbeddingManager(Info, _databaseManager);
    }

    public void Add(string token, double[] embedding, string tableName, EmbeddingType type)
    {
        _embeddingManager.Add(token, embedding, tableName, type);
    }

    public async Task BuildTable(List<string> tokens, string tableName, bool isVocabulary = false)
    {
        await _embeddingManager.BuildTable(tokens, tableName, isVocabulary);
    }

    public async Task<double[]> GetEmbedding(string token, EmbeddingType type)
    {
        return await _embeddingManager.GetEmbedding(token, type);
    }

    public void Save(string fileName)
    {
        _databaseManager.Save(fileName);
    }

    public void Load(string fileName)
    {
        _databaseManager.Load(fileName);
    }

    public void Clear()
    {
        _databaseManager.Clear();
    }

    public void CreateTable(string tableName)
    {
        _databaseManager.CreateTable(tableName);
    }

    public void RemoveTable(string tableName)
    {
        _databaseManager.DeleteTable(tableName);
    }

    public void ListTables()
    {
        _databaseManager.ListTables();
    }

    public Dictionary<string, List<Embedding>> GetTables()
    {
        return _databaseManager.GetTables();
    }

    public Dictionary<string, Embedding> GetTable(string tableName)
    {
        if (_databaseManager.GetTables().TryGetValue(tableName, out var table))
        {
            return table.ToDictionary(e => e.Token, e => e);
        }
        return null;
    }

    public string GetInfo()
    {
        return Info.Get();
    }

    public int GenerateUniqueId()
    {
        return _currentId++;
    }

    public void ViewTable(string tableName)
    {
        _databaseManager.ViewTable(tableName);
    }

    public List<(string TableName, Embedding Embedding)> FindInTables(string token, string tableName = TableNames.Vocabulary, bool searchAllTables = false)
    {
        var finder = new DatabaseFinder(_databaseManager.GetTables());
        return finder.FindInTables(token, tableName, searchAllTables);
    }

    public Embedding Match(double[] vector)
    {
        return _embeddingManager.Match(vector);
    }

    public bool HasTable(string tableName)
    {
        return _databaseManager.GetTables().ContainsKey(tableName);
    }

    public bool IsLoaded()
    {
        return _databaseManager.IsLoaded();
    }

    public double[] FindEmbedding(string tableName, string token)
    {
        return _databaseManager.FindEmbedding(tableName, token);
    }
}
