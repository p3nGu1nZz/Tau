using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class SaveLoadManager
{
    private readonly Dictionary<string, List<Embedding>> _tables;
    private readonly DatabaseInfo _info;

    public SaveLoadManager(Dictionary<string, List<Embedding>> tables, DatabaseInfo info)
    {
        _tables = tables;
        _info = info;
    }

    public void Save(string fileName = Constants.DatabaseFileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            fileName = Constants.DatabaseFileName;
        }

        string filePath = DataUtilities.GetFilePath(fileName);
        Log.Message($"Saving database to {fileName}");

        var stopwatch = Stopwatch.StartNew();

        var embeddingsList = new EmbeddingsList
        {
            version = _info.Version,
            model_name = _info.ModelName,
            organization = _info.Organization,
            total_embeddings = _info.TotalEmbeddings,
            tables = _tables
        };

        var content = DatabaseSerializer.Serialize(embeddingsList);
        DatabaseUtilities.CompressStringToFile(content, filePath);

        Log.Message($"Total embeddings in database: {_info.TotalEmbeddings}");

        stopwatch.Stop();

        Log.Message($"Database saved to {filePath} (Elapsed time: {stopwatch.Elapsed.TotalSeconds} seconds)");
    }

    public void Load(string fileName = Constants.DatabaseFileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            fileName = Constants.DatabaseFileName;
        }

        string filePath = DataUtilities.GetFilePath(fileName);
        Log.Message($"Loading database from {filePath}");

        var stopwatch = Stopwatch.StartNew();

        string content = DatabaseUtilities.ExtractToString(filePath);
        if (string.IsNullOrEmpty(content))
        {
            Log.Error("Failed to load custom format from file.");
            return;
        }

        Log.Message("Custom format loaded successfully. Deserializing...");

        var embeddingsList = DatabaseDeserializer.Deserialize(content);
        if (embeddingsList == null)
        {
            Log.Error("Failed to deserialize custom format into EmbeddingsList.");
            return;
        }

        Log.Message($"Database version: {embeddingsList.version}");
        Log.Message($"Model Name: {embeddingsList.model_name}");
        Log.Message($"Organization: {embeddingsList.organization}");
        Log.Message($"Total Embeddings: {embeddingsList.total_embeddings}");

        if (embeddingsList.tables == null)
        {
            Log.Error("EmbeddingsList.tables is null.");
            return;
        }

        Log.Message("Clearing existing tables.");
        _tables.Clear();

        foreach (var table in embeddingsList.tables)
        {
            if (table.Value == null)
            {
                Log.Error($"Table {table.Key} is null.");
                continue;
            }

            Log.Message($"Adding table {table.Key} with {table.Value.Count} embeddings.");
            _tables[table.Key] = table.Value;
        }

        _info.TotalEmbeddings = embeddingsList.total_embeddings;
        Log.Message($"Total embeddings set to {_info.TotalEmbeddings}");
        _info.DatabaseSize = embeddingsList.tables.Values.Sum(table => table?.Sum(e => e.Vector.Count * sizeof(double)) ?? 0);
        _info.TableCount = _tables.Count;

        stopwatch.Stop();
        Log.Message($"Vector Database loaded successfully from {fileName} (Elapsed time: {stopwatch.Elapsed.TotalSeconds} seconds)");
    }
}
