using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class FinderUtilities
{
    public static int CalculateColumnWidth(string header, IEnumerable<string> values, int maxWidth, int maxLength = int.MaxValue)
    {
        int headerWidth = header.Length + 2;
        int valuesWidth = values.Max(v => Math.Min(v.Length, maxLength)) + 2;
        return Math.Min(Math.Max(headerWidth, valuesWidth), maxWidth);
    }

    public static string FormatHeader(int idColumnWidth, int typeColumnWidth, int tokenColumnWidth, int tableColumnWidth, int embeddingColumnWidth)
    {
        return $"| {"Id".PadRight(idColumnWidth)} | {"Type".PadRight(typeColumnWidth)} | {"Token".PadRight(tokenColumnWidth)} | {"Table".PadRight(tableColumnWidth)} | {"Embedding (first 3)".PadRight(embeddingColumnWidth)} |";
    }

    public static string TruncateText(string text, int maxLength)
    {
        return text.Length > maxLength ? text.Substring(0, maxLength) + Constants.TextElipsis : text;
    }

    public static string FormatEmbeddings(List<double> vector, int numEmbeddingsToShow)
    {
        return string.Join(Constants.VectorSeparator, vector.Take(numEmbeddingsToShow)) + (vector.Count > numEmbeddingsToShow ? Constants.TextElipsis : "");
    }

    public static string FormatRecord(List<string> idLines, List<string> typeLines, List<string> tokenLines, List<string> tableLines, List<string> embeddingLines, int idColumnWidth, int typeColumnWidth, int tokenColumnWidth, int tableColumnWidth, int embeddingColumnWidth)
    {
        var result = new StringBuilder();
        for (int i = 0; i < idLines.Count; i++)
        {
            string idLine = i < idLines.Count ? idLines[i] : "";
            string typeLine = i < typeLines.Count ? typeLines[i] : "";
            string tokenLine = i < tokenLines.Count ? tokenLines[i] : "";
            string tableLine = i < tableLines.Count ? tableLines[i] : "";
            string embeddingLine = i < embeddingLines.Count ? embeddingLines[i] : "";
            result.AppendLine($"| {idLine.PadRight(idColumnWidth)} | {typeLine.PadRight(typeColumnWidth)} | {tokenLine.PadRight(tokenColumnWidth)} | {tableLine.PadRight(tableColumnWidth)} | {embeddingLine.PadRight(embeddingColumnWidth)} |");
        }
        result.Append($"| {"".PadRight(idColumnWidth)} | {"".PadRight(typeColumnWidth)} | {"".PadRight(tokenColumnWidth)} | {"".PadRight(tableColumnWidth)} | {"".PadRight(embeddingColumnWidth)} |");
        return result.ToString();
    }
}
