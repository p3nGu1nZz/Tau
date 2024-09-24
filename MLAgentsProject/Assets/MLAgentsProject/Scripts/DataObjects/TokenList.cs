using System.Collections.Generic;

[System.Serializable]
public class TokenList
{
    public string version;
    public string model_name;
    public string organization;
    public List<Token> token_data;

    public TokenList()
    {
        token_data = new List<Token>();
    }
}
