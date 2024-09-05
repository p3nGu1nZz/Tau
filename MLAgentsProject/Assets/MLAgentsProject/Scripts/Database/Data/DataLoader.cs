using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public static class DataLoader
{
    private const string Version = "0.1.0";
    private const string ModelName = "Tau-LLM";
    private const string Organization = "Tau";

    public static async Task LoadData(MessageList messageList)
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
        var file = DataUtilities.GetFilePath(fileName);
        if (File.Exists(file))
        {
            string jsonData = File.ReadAllText(file);
            MessageList messageList = Deserialize(jsonData);

            if (messageList != null)
            {
                RemoveDuplicates(messageList);
            }

            return messageList;
        }
        else
        {
            Log.Error($"{fileName} file not found.");
            return null;
        }
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

    public static void RemoveDuplicates(MessageList messageList)
    {
        var trainingDuplicates = MessageComparer.FindDuplicates(messageList.training_data);
        var evaluationDuplicates = MessageComparer.FindDuplicates(messageList.evaluation_data);

        messageList.training_data = messageList.training_data.Except(trainingDuplicates).ToList();
        messageList.evaluation_data = messageList.evaluation_data.Except(evaluationDuplicates).ToList();

        // Remove messages from evaluation_data if they exist in training_data
        var trainingMessages = new HashSet<string>(messageList.training_data.SelectMany(m => m.turns.Where(t => t.role == "User").Select(t => t.message)));
        int initialEvaluationCount = messageList.evaluation_data.Count;
        messageList.evaluation_data = messageList.evaluation_data.Where(m => !trainingMessages.Contains(m.turns.FirstOrDefault(t => t.role == "User")?.message)).ToList();
        int messagesRemovedFromEvaluation = initialEvaluationCount - messageList.evaluation_data.Count;

        Log.Message($"Training data duplicates removed: {trainingDuplicates.Count}");
        Log.Message($"Evaluation data duplicates removed: {evaluationDuplicates.Count}");
        Log.Message($"Messages removed from evaluation data because they exist in training data: {messagesRemovedFromEvaluation}");
    }

    public static void Shuffle(MessageList messageList)
    {
        System.Random rng = new();
        messageList.training_data = messageList.training_data.OrderBy(_ => rng.Next()).ToList();
        messageList.evaluation_data = messageList.evaluation_data.OrderBy(_ => rng.Next()).ToList();

        Log.Message("Training data and evaluation data have been randomized.");
    }
}
