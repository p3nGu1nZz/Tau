using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        Log.Message($"Added {words.Length} words to vocabulary.");
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
        Log.Message($"Normalized text: {text}");

        return text;
    }
}
