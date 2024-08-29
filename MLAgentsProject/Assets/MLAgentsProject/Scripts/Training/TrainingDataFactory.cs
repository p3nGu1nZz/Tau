using System.Collections.Generic;
using System.IO;

public static class TrainingDataFactory
{
    public static List<EmbeddingPair> CreateTrainingData(string fileName)
    {
        Log.Message($"Creating training data list from file: {fileName}");
        var trainingDataList = new List<EmbeddingPair>();
        var messageList = Load(fileName);

        foreach (var message in messageList.training_data)
        {
            try
            {
                Log.Message($"Processing message with {message.turns.Count} turns.");
                CheckTurns(message.turns);

                for (int i = 0; i < message.turns.Count - 1; i += 2)
                {
                    var userTurn = message.turns[i];
                    var agentTurn = message.turns[i + 1];

                    Log.Message($"User turn: {userTurn.message}, Agent turn: {agentTurn.message}");
                    var inputEmbedding = GetEmbedding(userTurn.message);
                    var outputEmbedding = GetEmbedding(agentTurn.message);
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

    private static void CheckTurns(List<Turn> turns)
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

    private static MessageList Load(string fileName)
    {
        Log.Message($"Loading message list from file: {fileName}");
        string filePath = DataUtilities.GetFilePath(fileName);
        return DataLoader.Load(filePath);
    }

    private static double[] GetEmbedding(string token)
    {
        Log.Message($"Getting embedding for token: {token}");
        // Implement the logic to get the embedding for the given token
        // This is a placeholder implementation
        // This should also actually search the database for the token in our training_data table.
        // replace our random vector with that found embedding from the token.
        return VectorUtilities.GetRandomVector(DatabaseConstants.VectorSize);
    }
}
