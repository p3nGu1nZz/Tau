using System;

[Serializable]
public class Response
{
    public string prompt;
    public string response;
    public bool is_valid;
    public string domain;
    public string context;
    public string reason;
}
