using System.Collections.Generic;
using System.Threading.Tasks;

public class EmbeddingManager
{
    private DatabaseManager _tableManager;
    private EmbeddingStorage _embeddingStorage;
    private TableBuilder _tableBuilder;

    public EmbeddingManager(DatabaseInfo info, DatabaseManager tableManager)
    {
        _tableManager = tableManager;
        _embeddingStorage = new EmbeddingStorage(info, tableManager);
        _tableBuilder = new TableBuilder(_embeddingStorage, _tableManager);
        Log.Message("EmbeddingManager initialized.");
    }

    public void Add(string token, double[] embedding, string tableName, EmbeddingType type)
    {
        _embeddingStorage.Add(token, embedding, tableName, type);
    }

    public async Task<double[]> GenerateEmbedding(string token, EmbeddingType type)
    {
        return await GenerateEmbeddingTask.Execute(token, type);
    }

    public async Task<double[]> GetEmbedding(string token, EmbeddingType type)
    {
        return await _embeddingStorage.GetEmbedding(token, type);
    }

    public async Task BuildTable(List<string> tokens, string tableName, bool isVocabulary = false)
    {
        await _tableBuilder.BuildTable(tokens, tableName, isVocabulary);
    }

    public async Task BuildTokenTable(string filename)
    {
        await _tableBuilder.BuildTokenTable(filename);
    }

    public Embedding Match(double[] vector, EmbeddingType type)
    {
        return _embeddingStorage.Match(vector, type);
    }
}
