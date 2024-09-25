using CommandTerminal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class DataUtilities
{
    public static string GetFilePath(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentNullException(nameof(fileName), "File name cannot be null or empty.");
        }

        return Path.Combine(Application.dataPath, Constants.UpDirectoryLevel, Constants.DataDirectoryName, fileName);
    }

    public static string CombineArgs(CommandArg[] args)
    {
        return string.Join(" ", args.Select(arg => arg.String));
    }

    public static List<string> ParseFileNames(string combinedArgs)
    {
        if (string.IsNullOrEmpty(combinedArgs))
        {
            throw new ArgumentException("Combined arguments cannot be null or empty.");
        }

        return combinedArgs.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    public static List<string> GetDirectoryContents(string directoryPath, string searchPattern)
    {
        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException($"Directory '{directoryPath}' not found.");
        }

        return Directory.GetFiles(directoryPath, searchPattern).Select(Path.GetFileName).ToList();
    }

    public static void Shuffle(MessageList messageList)
    {
        System.Random rng = new();
        messageList.training_data = messageList.training_data.OrderBy(_ => rng.Next()).ToList();
        messageList.evaluation_data = messageList.evaluation_data.OrderBy(_ => rng.Next()).ToList();

        Log.Message("Training data and evaluation data have been randomized.");
    }

    public static void CleanPunctuationSpaces(MessageList messageList)
    {
        Regex pattern = new(@"([?.!])\s+$");
        CleanMessageList(messageList.training_data, pattern);
        CleanMessageList(messageList.evaluation_data, pattern);
    }

    public static void CleanMessageList(List<Message> messages, Regex pattern)
    {
        foreach (var message in messages)
        {
            foreach (var turn in message.turns)
            {
                turn.message = pattern.Replace(turn.message, "$1");
            }
        }
    }

    public static void PruneMessages(List<Message> messages, string tableName)
    {
        Log.Message($"Pruning messages for table: {tableName}");
        Log.Message($"Initial message count: {messages.Count}");

        for (int i = messages.Count - 1; i >= 0; i--)
        {
            Message message = messages[i];
            foreach (var turn in message.turns)
            {
                double[] result = Database.Instance.FindEmbedding(tableName, turn.message);
                if (result == null)
                {
                    Log.Message($"Pruning message: {turn.message}");
                    messages.RemoveAt(i);
                    break;
                }
            }
        }

        Log.Message($"Post-pruning message count: {messages.Count}");
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
}
