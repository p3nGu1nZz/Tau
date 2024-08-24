using System.Collections.Generic;

public enum DatabaseTokens
{
    Database,
    Version,
    Model_Name,
    Organization,
    Total_Embeddings,
    Table,
    Embedding,
    Id,
    Token,
    Type,
    Vector,
    End
}

public static class DatabaseStrings
{
    public static readonly Dictionary<DatabaseTokens, string> Tokens = new Dictionary<DatabaseTokens, string>
    {
        { DatabaseTokens.Database, "<|database|>" },
        { DatabaseTokens.Version, "<|version|>" },
        { DatabaseTokens.Model_Name, "<|model_name|>" },
        { DatabaseTokens.Organization, "<|organization|>" },
        { DatabaseTokens.Total_Embeddings, "<|total_embeddings|>" },
        { DatabaseTokens.Table, "<|table|>" },
        { DatabaseTokens.Embedding, "<|embedding|>" },
        { DatabaseTokens.Id, "<|id|>" },
        { DatabaseTokens.Token, "<|token|>" },
        { DatabaseTokens.Type, "<|type|>" },
        { DatabaseTokens.Vector, "<|vector|>" },
        { DatabaseTokens.End, "<|end|>" }
    };
}
