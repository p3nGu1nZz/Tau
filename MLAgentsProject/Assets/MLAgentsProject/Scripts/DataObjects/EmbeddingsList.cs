using System;
using System.Collections.Generic;

[Serializable]
public class EmbeddingsList
{
    public string version;
    public string model_name;
    public string organization;
    public int total_embeddings;
    public Dictionary<string, List<Embedding>> tables;
}
