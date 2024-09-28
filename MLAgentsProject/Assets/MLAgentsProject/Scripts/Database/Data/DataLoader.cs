using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public static class DataLoader
{
    public static async Task LoadData(MessageList messageList)
    {
        List<string> vocabulary = ExtractAndLogVocabulary(messageList);
        var (chunkedTrainingData, combinedTrainingData) = ProcessAndLogData(messageList.training_data, "training");
        var (chunkedEvaluationData, combinedEvaluationData) = ProcessAndLogData(messageList.evaluation_data, "evaluation");

        await DatabaseUtilities.BuildAndPopulateTables(vocabulary, combinedTrainingData, combinedEvaluationData);
        await DatabaseUtilities.AggregateAndStoreEmbeddings(chunkedTrainingData, chunkedEvaluationData);

        string tokenFileName = await Tokenizer.Export(vocabulary);
        await Database.Instance.BuildTokenTable(tokenFileName);

        LogTableInfo();
    }

    public static MessageList Load(string fileName)
    {
        var file = DataUtilities.GetFilePath(fileName);
        if (File.Exists(file))
        {
            string jsonData = File.ReadAllText(file);
            MessageList messageList = Deserialize(jsonData);

            if (messageList != null)
            {
                DataUtilities.RemoveDuplicates(messageList);
            }

            return messageList;
        }
        else
        {
            Log.Error($"{fileName} file not found.");
            return null;
        }
    }

    public static MessageList Prune(MessageList messageList)
    {
        Log.Message("Starting pruning process...");
        Log.Message($"Initial training data count: {messageList.training_data.Count}");
        Log.Message($"Initial evaluation data count: {messageList.evaluation_data.Count}");

        DataUtilities.CleanPunctuationSpaces(messageList);
        Log.Message("Cleaned punctuation and spaces.");

        Log.Message("Pruning training data...");
        DataUtilities.PruneMessages(messageList.training_data, TableNames.TrainingData);
        Log.Message($"Post-pruning training data count: {messageList.training_data.Count}");

        Log.Message("Pruning evaluation data...");
        DataUtilities.PruneMessages(messageList.evaluation_data, TableNames.EvaluationData);
        Log.Message($"Post-pruning evaluation data count: {messageList.evaluation_data.Count}");

        return messageList;
    }

    private static MessageList Deserialize(string jsonData)
    {
        return JsonUtility.FromJson<MessageList>(jsonData);
    }

    public static void Save(MessageList messageList, string fileName)
    {
        var file = DataUtilities.GetFilePath(fileName);
        string jsonData = JsonUtility.ToJson(messageList, true);
        File.WriteAllText(file, jsonData);
        Log.Message($"Data saved to {fileName}");
    }

    private static List<string> ExtractAndLogVocabulary(MessageList messageList)
    {
        List<Message> combinedData = new List<Message>();
        combinedData.AddRange(messageList.training_data);
        combinedData.AddRange(messageList.evaluation_data);

        List<string> vocabulary = Tokenizer.ExtractVocabulary(combinedData);
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

        Dictionary<string, List<string>> chunkedData = TokenizerUtilities.ChunkText(dataList);
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
