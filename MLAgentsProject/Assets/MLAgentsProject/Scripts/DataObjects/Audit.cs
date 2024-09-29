using System.Collections.Generic;

[System.Serializable]
public class Audit
{
    public string version;
    public string model_name;
    public string organization;
    public int total_messages;
    public int missing_embeddings;
    public int created_embeddings;
    public List<string> missing_tokens;

    public Audit()
    {
        missing_tokens = new List<string>();
    }
}
