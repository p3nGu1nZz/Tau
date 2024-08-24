using System.Collections.Generic;
using static TauAgent;

[System.Serializable]
public class Message
{
    public string domain;
    public string context;
    public string system;
    public List<Turn> turns;
}
