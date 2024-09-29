public static class Constants
{
    public const string Version = "0.1.0";
    public const string ModelName = "Tau";
    public const string Organization = "Tau";

    public static readonly string[] ReservedWords = new string[]
    {
        "domain", "context", "system", "user", "agent", "turns", "role", "version", "model_name", "organization", "training_data", "evaluation_data", "data", "id", "type"
    };

    public const string DefaultDataFileName = "data.json";
    public const string DatabaseFileName = "database.bin";
    public const string ScriptsDirectoryName = "Scripts";
    public const string DataDirectoryName = "Data";
    public const string UpDirectoryLevel = "..";
    public const string VectorSeparator = "|";
    public const string TextElipsis = "...";
    public const char TableSeparator = '-';
    public const int MaxTableWidth = 110;
    public const int MaxLogLength = 110;
    public const int OverlapTokens = 16;
    public const int VectorSize = 384;
    public const int MaxTokens = 128;
    public const int TokenSize = 3;
}
