using System;

public class DatabaseInfo
{
    public string Version { get; set; }
    public string ModelName { get; set; }
    public string Organization { get; set; }
    public int TotalEmbeddings { get; set; }
    public long DatabaseSize { get; set; }
    public int TableCount { get; set; }

    public DatabaseInfo()
    {
        Version = "0.1.0";
        ModelName = "Tau";
        Organization = "Huggingface";
        TotalEmbeddings = 0;
        DatabaseSize = 0;
        TableCount = 0;
    }

    public string Get()
    {
        if (TotalEmbeddings == 0)
        {
            return "Not Initialized";
        }
        else
        {
            return $"Vector Database Info:\n" +
                   $"Version: {Version}\n" +
                   $"Model Name: {ModelName}\n" +
                   $"Organization: {Organization}\n" +
                   $"Total Embeddings: {TotalEmbeddings}\n" +
                   $"Database Size: {DatabaseSize} bytes\n" +
                   $"Table Count: {TableCount}";
        }
    }
}
