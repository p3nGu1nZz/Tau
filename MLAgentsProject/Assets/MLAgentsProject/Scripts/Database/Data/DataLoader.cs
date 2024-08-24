using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public static class DataLoader
{
    private const string Version = "0.1.0";
    private const string ModelName = "Tau";
    private const string Organization = "Huggingface";

    public static async Task ConcatenateAndLoadData(List<string> fileNames, string outputFileName)
    {
        var combinedMessageList = new MessageList
        {
            version = Version,
            model_name = ModelName,
            organization = Organization,
            training_data = new List<Message>(),
            evaluation_data = new List<Message>()
        };

        foreach (var fileName in fileNames)
        {
            var messageList = Load(fileName);
            if (messageList != null)
            {
                combinedMessageList.training_data.AddRange(messageList.training_data);
                combinedMessageList.evaluation_data.AddRange(messageList.evaluation_data);
            }
        }

        Save(combinedMessageList, outputFileName);
        Log.Message($"Combined data saved to {outputFileName}");

        await LoadData(combinedMessageList, outputFileName);
    }

    public static async Task LoadData(MessageList messageList, string fileName)
    {
        List<string> vocabulary = ExtractAndLogVocabulary(messageList);
        var (chunkedTrainingData, combinedTrainingData) = ProcessAndLogData(messageList.training_data, "training");
        var (chunkedEvaluationData, combinedEvaluationData) = ProcessAndLogData(messageList.evaluation_data, "evaluation");

        Tokenizer.Export(TableNames.Vocabulary, vocabulary, Version, ModelName, Organization);

        await DatabaseUtilities.BuildAndPopulateTables(vocabulary, combinedTrainingData, combinedEvaluationData);
        await DatabaseUtilities.AggregateAndStoreEmbeddings(chunkedTrainingData, chunkedEvaluationData);

        LogTableInfo();
    }

    public static MessageList Load(string fileName)
    {
        var file = DatabaseUtilities.GetFilePath(fileName);
        if (File.Exists(file))
        {
            string jsonData = File.ReadAllText(file);
            return Deserialize(jsonData);
        }
        else
        {
            Debug.LogError($"{fileName} file not found.");
            return null;
        }
    }

    private static MessageList Deserialize(string jsonData)
    {
        return JsonUtility.FromJson<MessageList>(jsonData);
    }

    public static void Save(MessageList messageList, string fileName)
    {
        var file = DatabaseUtilities.GetFilePath(fileName);
        string jsonData = JsonUtility.ToJson(messageList, true);
        File.WriteAllText(file, jsonData);
        Debug.Log($"Combined data saved to {fileName}");
    }

    private static List<string> ExtractAndLogVocabulary(MessageList messageList)
    {
        List<Message> combinedData = new List<Message>();
        combinedData.AddRange(messageList.training_data);
        combinedData.AddRange(messageList.evaluation_data);

        List<string> vocabulary = DatabaseUtilities.ExtractVocabulary(combinedData);
        Log.Message($"Vocabulary size: {vocabulary.Count}");
        return vocabulary;
    }

    private static (Dictionary<string, List<string>> chunkedData, List<string> combinedData) ProcessAndLogData(List<Message> data, string dataType)
    {
        List<string> dataList = data.SelectMany(m => m.turns.Select(t => t.message.ToLower())).ToList();
        Log.Message($"Extracted {dataType} data size: {dataList.Count}");
        if (dataList.Count > 0)
        {
            Log.Message($"Sample {dataType} message: {dataList[0]}");
        }

        Dictionary<string, List<string>> chunkedData = Tokenizer.ChunkText(dataList);
        List<string> combinedData = chunkedData.Values.SelectMany(chunks => chunks).ToList();

        // Include short texts that were not chunked
        var shortTexts = dataList.Where(text => !chunkedData.ContainsKey(text)).ToList();
        combinedData.AddRange(shortTexts);

        Log.Message($"Chunked {dataType} data size: {chunkedData.Values.Sum(chunks => chunks.Count)}");
        Log.Message($"Total {dataType} data size (including short texts): {combinedData.Count}");
        return (chunkedData, combinedData);
    }

    private static void LogTableInfo()
    {
        var tableNames = string.Join(", ", Database.Instance.GetTables().Keys);
        Log.Message($"Total tables exported: {Database.Instance.GetTables().Count}");
        Log.Message($"Table names: {tableNames}");
    }
}
