using System.Collections.Generic;

[System.Serializable]
public class TokenList
{
    public string version;
    public string model_name;
    public string organization;
    public int total_tokens;
    public List<Token> tokens;

    public TokenList()
    {
        tokens = new List<Token>();
    }
}
