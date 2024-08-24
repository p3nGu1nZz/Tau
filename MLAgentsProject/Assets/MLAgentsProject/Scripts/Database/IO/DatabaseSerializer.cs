using System.Collections.Generic;
using System.Text;

public class DatabaseSerializer
{
    public static string Serialize(EmbeddingsList embeddingsList)
    {
        StringBuilder sb = new StringBuilder();

        Log.Message("Starting serialization of database info.");

        // Serialize database info
        sb.AppendLine(DatabaseStrings.Tokens[DatabaseTokens.Database]);
        sb.AppendLine(DatabaseStrings.Tokens[DatabaseTokens.Version]);
        sb.AppendLine(embeddingsList.version);
        sb.AppendLine(DatabaseStrings.Tokens[DatabaseTokens.End]);
        sb.AppendLine(DatabaseStrings.Tokens[DatabaseTokens.Model_Name]);
        sb.AppendLine(embeddingsList.model_name);
        sb.AppendLine(DatabaseStrings.Tokens[DatabaseTokens.End]);
        sb.AppendLine(DatabaseStrings.Tokens[DatabaseTokens.Organization]);
        sb.AppendLine(embeddingsList.organization);
        sb.AppendLine(DatabaseStrings.Tokens[DatabaseTokens.End]);
        sb.AppendLine(DatabaseStrings.Tokens[DatabaseTokens.Total_Embeddings]);
        sb.AppendLine(embeddingsList.total_embeddings.ToString());
        sb.AppendLine(DatabaseStrings.Tokens[DatabaseTokens.End]);

        Log.Message("Database info serialized successfully.");

        // Serialize tables
        SerializeTables(sb, embeddingsList.tables);

        Log.Message("Serialization complete.");
        return sb.ToString();
    }

    private static void SerializeTables(StringBuilder sb, Dictionary<string, List<Embedding>> tables)
    {
        foreach (var table in tables)
        {
            SerializeTable(sb, table.Key, table.Value);
        }
    }

    private static void SerializeTable(StringBuilder sb, string tableName, List<Embedding> embeddings)
    {
        Log.Message($"Serializing table: {tableName}, Embeddings count: {embeddings.Count}");

        sb.AppendLine(DatabaseStrings.Tokens[DatabaseTokens.Table]);
        sb.AppendLine(tableName);
        foreach (var embedding in embeddings)
        {
            sb.AppendLine(DatabaseStrings.Tokens[DatabaseTokens.Embedding]);
            sb.AppendLine(DatabaseStrings.Tokens[DatabaseTokens.Id]);
            sb.AppendLine(embedding.Id.ToString());
            sb.AppendLine(DatabaseStrings.Tokens[DatabaseTokens.End]);
            sb.AppendLine(DatabaseStrings.Tokens[DatabaseTokens.Token]);
            sb.AppendLine(embedding.Token);
            sb.AppendLine(DatabaseStrings.Tokens[DatabaseTokens.End]);
            sb.AppendLine(DatabaseStrings.Tokens[DatabaseTokens.Type]);
            sb.AppendLine(embedding.Type.ToString());
            sb.AppendLine(DatabaseStrings.Tokens[DatabaseTokens.End]);
            sb.AppendLine(DatabaseStrings.Tokens[DatabaseTokens.Vector]);
            sb.AppendLine($"[{string.Join("|", embedding.Vector)}]");
            sb.AppendLine(DatabaseStrings.Tokens[DatabaseTokens.End]);
            sb.AppendLine(DatabaseStrings.Tokens[DatabaseTokens.End]);
        }
        sb.AppendLine(DatabaseStrings.Tokens[DatabaseTokens.End]);

        Log.Message($"Table {tableName} serialized successfully.");
    }
}
