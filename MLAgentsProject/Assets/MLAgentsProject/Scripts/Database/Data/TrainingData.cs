using System.IO;
using UnityEngine;

public class TrainingData
{
    private static string filePath = Path.Combine(Application.dataPath, "..", "Data");

    public static MessageList Load(string fileName)
    {
        var file = Path.Combine(filePath, fileName);
        if (File.Exists(file))
        {
            string jsonData = File.ReadAllText(file);
            return Deserialize(jsonData);
        }
        else
        {
            Debug.LogError("training_data.json file not found.");
            return null;
        }
    }

    private static MessageList Deserialize(string jsonData)
    {
        return JsonUtility.FromJson<MessageList>(jsonData);
    }
}
