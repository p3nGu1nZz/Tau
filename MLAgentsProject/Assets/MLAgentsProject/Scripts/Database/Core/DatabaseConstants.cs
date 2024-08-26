public static class DatabaseConstants
{
    public static readonly string[] ReservedWords = new string[]
    {
        "domain", "context", "system", "user", "agent", "turns", "role", "version", "model_name", "organization", "training_data", "evaluation_data", "data", "id", "type"
    };

    public const string DatabaseFileName = "database.bin";
    public const string DataDirectoryName = "Data";
    public const string UpDirectoryLevel = "..";
    public const string VectorSeparator = "|";
    public const string TextElipsis = "...";
    public const char TableSeparator = '-';
    public const int MaxTableWidth = 110;
    public const int MaxLogLength = 110;
    public const int VectorSize = 384;
}
