using System;
using System.Collections.Generic;
using System.Linq;

public static class StringUtilities
{
    public static List<string> WrapText(string text, int maxWidth, int maxLines)
    {
        List<string> lines = new List<string>();
        int start = 0;

        while (start < text.Length && lines.Count < maxLines)
        {
            int length = Math.Min(maxWidth, text.Length - start);
            if (start + length < text.Length)
            {
                length -= 3;
            }
            lines.Add(text.Substring(start, length));
            start += length;
        }

        if (start < text.Length)
        {
            lines[lines.Count - 1] = lines[lines.Count - 1].TrimEnd() + Constants.TextElipsis;
        }

        return lines;
    }

    public static string ExtractQuotedString(ref string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentException("Input cannot be null or empty.");
        }

        int startQuoteIndex = input.IndexOf('"');
        if (startQuoteIndex == -1)
        {
            throw new ArgumentException("No starting quote found.");
        }

        int endQuoteIndex = input.IndexOf('"', startQuoteIndex + 1);
        if (endQuoteIndex == -1)
        {
            throw new ArgumentException("No ending quote found.");
        }

        string quotedString = input.Substring(startQuoteIndex + 1, endQuoteIndex - startQuoteIndex - 1);
        input = input.Substring(endQuoteIndex + 1).Trim();

        return quotedString;
    }

    public static string TruncateLogMessage(string message)
    {
        if (string.IsNullOrEmpty(message) || message.Length <= Constants.MaxLogLength)
        {
            return message;
        }

        return message.Substring(0, Constants.MaxLogLength) + Constants.TextElipsis;
    }

    public static string ConvertVectorToString(double[] vector)
    {
        if (vector.Length != Constants.VectorSize)
        {
            throw new ArgumentException($"Vector must be of size {Constants.VectorSize}.");
        }

        return "[" + string.Join(Constants.VectorSeparator, vector.Select(v => v.ToString("F6"))) + "]";
    }

    public static string ConvertVectorToString(float[] vector)
    {
        if (vector.Length != Constants.VectorSize)
        {
            throw new ArgumentException($"Vector must be of size {Constants.VectorSize}.");
        }

        return "[" + string.Join(Constants.VectorSeparator, vector.Select(v => v.ToString("F6"))) + "]";
    }

    public static double[] ConvertStringToVector(string vectorString)
    {
        if (!vectorString.StartsWith("[") || !vectorString.EndsWith("]"))
        {
            throw new ArgumentException("Embedding vector must be enclosed in square brackets.");
        }

        string[] parts = vectorString.Trim('[', ']').Split(Constants.VectorSeparator);
        double[] vector = parts.Select(double.Parse).ToArray();

        if (vector.Length != Constants.VectorSize)
        {
            throw new ArgumentException($"Embedding vector must be of size {Constants.VectorSize}.");
        }

        return vector;
    }

    public static string TruncateVectorString(string vectorString, int maxLength = 80)
    {
        if (vectorString.Length <= maxLength)
        {
            return vectorString;
        }
        return vectorString.Substring(0, maxLength) + "...";
    }

    public static string CapitalizeFirstLetter(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToUpper(input[0]) + input[1..].ToLower();
    }

    public static string ScrubResponse(string response)
    {
        return response.Replace("\n", "").Replace("\r", "").Replace("\\", "").Replace("\"", "\\\"");
    }
}
