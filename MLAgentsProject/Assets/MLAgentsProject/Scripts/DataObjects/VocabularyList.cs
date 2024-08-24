using System.Collections.Generic;

[System.Serializable]
public class VocabularyList
{
    public string version;
    public string model_name;
    public string organization;
    public int total_words;
    public List<string> words;
}
