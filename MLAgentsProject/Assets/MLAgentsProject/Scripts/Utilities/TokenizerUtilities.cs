using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

public static class TokenizerUtilities
{
    public static void AddToVocabulary(string text, HashSet<string> vocabulary)
    {
        var words = Regex.Split(text, @"\s+");
        foreach (var word in words)
        {
            if (!string.IsNullOrWhiteSpace(word))
            {
                vocabulary.Add(word);
            }
        }
    }

    public static void AddToVocabulary(string[] words, HashSet<string> vocabulary)
    {
        foreach (var word in words)
        {
            if (!string.IsNullOrWhiteSpace(word))
            {
                vocabulary.Add(word);
            }
        }
        Log.Message($"Added {words.Length} words to vocabulary.");
    }

    public static string Normalize(string text)
    {
        text = text.ToLower();
        text = Regex.Replace(text, @"[^\w\s]", "");
        text = text.Trim();

        return text;
    }

    public static void ProcessWord(string word, Dictionary<string, Embedding> vocabulary, TokenList tokenList)
    {
        if (!vocabulary.TryGetValue(word, out var embedding))
        {
            throw new KeyNotFoundException($"Word '{word}' not found in vocabulary table.");
        }

        if (embedding.Vector.Count != Constants.VectorSize && embedding.Vector.Count != Constants.TokenSize)
        {
            throw new ArgumentException($"Embedding vector for word '{word}' must be of size {Constants.VectorSize} or {Constants.TokenSize}.");
        }

        tokenList.tokens.Add(new Token(embedding.Id, embedding.Token, embedding.Vector.ToArray()));
        Log.Message($"Token: {word}");
    }

    public static async Task SaveToFileAsync<T>(string fileName, T data)
    {
        using (StreamWriter writer = new(fileName))
        {
            string json = JsonUtility.ToJson(data, true);
            await writer.WriteAsync(json);
        }
    }

    public static List<Token> GetTokensFromVocabularyWords(Dictionary<string, Embedding> vocabulary, List<string> words)
    {
        var tokens = new List<Token>();
        foreach (var word in words)
        {
            if (vocabulary.TryGetValue(word, out var embedding))
            {
                if (embedding.Vector.Count != Constants.VectorSize && embedding.Vector.Count != Constants.TokenSize)
                {
                    throw new ArgumentException($"Embedding vector for word '{word}' must be of size {Constants.VectorSize} or {Constants.TokenSize}.");
                }
                tokens.Add(new Token(embedding.Id, embedding.Token, embedding.Vector.ToArray()));
            }
            else
            {
                throw new KeyNotFoundException($"Word '{word}' not found in vocabulary table.");
            }
        }
        return tokens;
    }

    public static Dictionary<string, List<string>> ChunkText(List<string> texts)
    {
        var chunkedTexts = new Dictionary<string, List<string>>();
        foreach (var text in texts)
        {
            var tokens = text.Split(' ');

            if (tokens.Length > Constants.MaxTokens)
            {
                List<string> chunks = new List<string>();
                for (int i = 0; i < tokens.Length; i += (Constants.MaxTokens - Constants.OverlapTokens))
                {
                    var chunk = tokens.Skip(i).Take(Constants.MaxTokens).ToArray();
                    chunks.Add(string.Join(" ", chunk));
                }
                chunkedTexts[text] = chunks;
            }
        }
        return chunkedTexts;
    }
}
