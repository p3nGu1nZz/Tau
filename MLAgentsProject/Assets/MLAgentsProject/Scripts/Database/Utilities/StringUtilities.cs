using System;
using System.Linq;

public static class StringUtilities
{
    public static string ExtractQuotedString(ref string input)
    {
        int startIndex = input.IndexOf('"');
        if (startIndex == -1) return null;

        int endIndex = input.IndexOf('"', startIndex + 1);
        if (endIndex == -1) return null;

        string result = input.Substring(startIndex + 1, endIndex - startIndex - 1);
        input = input.Substring(endIndex + 1).Trim();
        return result;
    }

    public static string ConvertVectorToString(double[] vector)
    {
        if (vector.Length != DatabaseConstants.VectorSize)
        {
            throw new ArgumentException($"Vector must be of size {DatabaseConstants.VectorSize}.");
        }

        return "[" + string.Join(DatabaseConstants.VectorSeparator, vector.Select(v => v.ToString("F6"))) + "]";
    }

    public static double[] ConvertStringToVector(string vectorString)
    {
        if (!vectorString.StartsWith("[") || !vectorString.EndsWith("]"))
        {
            throw new ArgumentException("Embedding vector must be enclosed in square brackets.");
        }

        string[] parts = vectorString.Trim('[', ']').Split(DatabaseConstants.VectorSeparator);
        double[] vector = parts.Select(double.Parse).ToArray();

        if (vector.Length != DatabaseConstants.VectorSize)
        {
            throw new ArgumentException($"Embedding vector must be of size {DatabaseConstants.VectorSize}.");
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
}
