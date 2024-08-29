using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class TrainingDataFactory
{
    public static List<EmbeddingPair> CreateTrainingDataList(string fileName)
    {
        Log.Message($"Creating training data list from file: {fileName}");
        var trainingDataList = new List<EmbeddingPair>();
        var messageList = LoadMessageList(fileName);

        foreach (var message in messageList.training_data)
        {
            try
            {
                Log.Message($"Processing message with {message.turns.Count} turns.");
                CheckMessages(message.turns);

                for (int i = 0; i < message.turns.Count - 1; i += 2)
                {
                    var userTurn = message.turns[i];
                    var agentTurn = message.turns[i + 1];

                    Log.Message($"User turn: {userTurn.message}, Agent turn: {agentTurn.message}");
                    var inputEmbedding = GetEmbeddingForToken(userTurn.message);
                    var outputEmbedding = GetEmbeddingForToken(agentTurn.message);
                    trainingDataList.Add(new EmbeddingPair(inputEmbedding, outputEmbedding));
                }
            }
            catch (InvalidDataException ex)
            {
                Log.Error($"Error processing message: {ex.Message}");
            }
        }

        Log.Message($"Created training data list with {trainingDataList.Count} embedding pairs.");
        return trainingDataList;
    }

    private static void CheckMessages(List<Turn> turns)
    {
        Log.Message("Checking message pairs.");
        if (turns.Count % 2 != 0)
        {
            throw new InvalidDataException("Invalid number of messages. Messages should be in pairs of user and agent.");
        }

        for (int i = 0; i < turns.Count; i += 2)
        {
            if (turns[i].role != "User" || turns[i + 1].role != "Agent")
            {
                throw new InvalidDataException("Invalid message order. Expected User followed by Agent.");
            }
        }
    }

    private static MessageList LoadMessageList(string fileName)
    {
        Log.Message($"Loading message list from file: {fileName}");
        string filePath = DataUtilities.GetFilePath(fileName);
        return DataLoader.Load(filePath);
    }

    private static double[] GetEmbeddingForToken(string token)
    {
        Log.Message($"Getting embedding for token: {token}");
        // Implement the logic to get the embedding for the given token
        // This is a placeholder implementation
        return new double[] { 0.0, 1.0, 0.0 }; // Replace with actual logic
    }
}
