using System.Collections.Generic;

[System.Serializable]
public class MessageList
{
    public string version;
    public string model_name;
    public string organization;
    public List<Message> training_data;
    public List<Message> evaluation_data;
}
