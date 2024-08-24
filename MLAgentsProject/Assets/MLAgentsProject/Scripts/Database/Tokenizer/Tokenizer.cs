using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class Tokenizer
{
    public static int MaxTokens { get; set; } = 128;
    public static int OverlapTokens { get; set; } = 16;
    private static HashSet<string> _vocabulary = new HashSet<string>();

    public static List<string> ExtractVocabulary(List<Message> messages)
    {
        _vocabulary.Clear();

        // Add reserved words to vocabulary
        Log.Message($"Adding '{DatabaseConstants.ReservedWords.Length}' reserved words to 'vocabulary'...");
        TokenizerUtilities.AddToVocabulary(DatabaseConstants.ReservedWords, _vocabulary);

        foreach (var message in messages)
        {
            Log.Message($"Processing message: {message.domain}");
            TokenizerUtilities.AddToVocabulary(TokenizerUtilities.Normalize(message.domain), _vocabulary);
            TokenizerUtilities.AddToVocabulary(TokenizerUtilities.Normalize(message.context), _vocabulary);
            TokenizerUtilities.AddToVocabulary(TokenizerUtilities.Normalize(message.system), _vocabulary);

            foreach (var turn in message.turns)
            {
                TokenizerUtilities.AddToVocabulary(TokenizerUtilities.Normalize(turn.message), _vocabulary);
            }
        }

        List<string> sortedVocabulary = _vocabulary.ToList();
        sortedVocabulary.Sort();
        Log.Message($"Extracted vocabulary size: {sortedVocabulary.Count}");

        return sortedVocabulary;
    }

    public static string Export(string fileName, List<string> vocabulary, string version, string modelName, string organization)
    {
        string scriptsDirectory = Path.Combine(Application.dataPath, "..", "Data");
        string vocabFileName = Path.Combine(scriptsDirectory, Path.GetFileNameWithoutExtension(fileName) + ".json");

        var vocabList = new VocabularyList
        {
            version = version,
            model_name = modelName,
            organization = organization,
            total_words = vocabulary.Count,
            words = vocabulary
        };

        var json = JsonUtility.ToJson(vocabList, true);
        File.WriteAllText(vocabFileName, json);
        Log.Message($"Vocabulary exported successfully to {vocabFileName}");

        return vocabFileName;
    }

    public static Dictionary<string, List<string>> ChunkText(List<string> texts)
    {
        var chunkedTexts = new Dictionary<string, List<string>>();
        foreach (var text in texts)
        {
            var tokens = text.Split(' ');

            if (tokens.Length > MaxTokens)
            {
                List<string> chunks = new List<string>();
                for (int i = 0; i < tokens.Length; i += (MaxTokens - OverlapTokens))
                {
                    var chunk = tokens.Skip(i).Take(MaxTokens).ToArray();
                    chunks.Add(string.Join(" ", chunk));
                }
                chunkedTexts[text] = chunks;
                Log.Message($"Chunked text into {chunks.Count} chunks.");
            }
            else
            {
                Log.Message($"Text is too short to be chunked: {text}");
            }
        }
        return chunkedTexts;
    }
}
