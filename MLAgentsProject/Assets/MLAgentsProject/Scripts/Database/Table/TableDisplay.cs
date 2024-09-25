using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TableDisplay
{
    private readonly Dictionary<string, List<Embedding>> _tables;

    public TableDisplay(Dictionary<string, List<Embedding>> tables)
    {
        _tables = tables;
    }

    public void DisplayTable(string tableName)
    {
        if (string.IsNullOrEmpty(tableName))
        {
            throw new ArgumentException("Table name cannot be null or empty.");
        }

        var table = GetTable(tableName);
        if (table == null)
        {
            throw new ArgumentException($"Table '{tableName}' does not exist.");
        }

        Log.Message(FormatTable(tableName, table));
    }

    private Dictionary<string, Embedding> GetTable(string tableName)
    {
        if (_tables.TryGetValue(tableName, out var table))
        {
            return table.ToDictionary(e => e.Token, e => e);
        }
        else
        {
            throw new ArgumentException($"Table '{tableName}' does not exist.");
        }
    }

    private string FormatTable(string tableName, Dictionary<string, Embedding> table)
    {
        const int numEmbeddingsToShow = 3;
        const int maxLinesPerRecord = 2;
        const int maxTokenLength = 30;

        int idColumnWidth = CalculateColumnWidth("Id", table.Values.Select(k => k.Id.ToString()), 10);
        int typeColumnWidth = CalculateColumnWidth("Type", table.Values.Select(k => k.Type.ToString()), 10);
        int tokenColumnWidth = CalculateColumnWidth("Token", table.Keys.Select(k => k), 30, maxTokenLength);
        int embeddingColumnWidth = Constants.MaxTableWidth - (idColumnWidth + typeColumnWidth + tokenColumnWidth + 10);

        string separator = new string(Constants.TableSeparator, Constants.MaxTableWidth + 3);
        var results = new StringBuilder();
        results.AppendLine(separator);
        results.AppendLine(FormatHeader(idColumnWidth, typeColumnWidth, tokenColumnWidth, embeddingColumnWidth));
        results.AppendLine(separator);

        foreach (var entry in table)
        {
            results.AppendLine(FormatTableRow(entry, idColumnWidth, typeColumnWidth, tokenColumnWidth, embeddingColumnWidth, numEmbeddingsToShow, maxLinesPerRecord, maxTokenLength));
            results.AppendLine(separator);
        }

        return results.ToString();
    }

    private int CalculateColumnWidth(string header, IEnumerable<string> values, int maxWidth, int maxLength = int.MaxValue)
    {
        int headerWidth = header.Length + 2;
        int valuesWidth = values.Max(v => Math.Min(v.Length, maxLength)) + 2;
        return Math.Min(Math.Max(headerWidth, valuesWidth), maxWidth);
    }

    private string FormatHeader(int idColumnWidth, int typeColumnWidth, int tokenColumnWidth, int embeddingColumnWidth)
    {
        return $"| {"Id".PadRight(idColumnWidth)} | {"Type".PadRight(typeColumnWidth)} | {"Token".PadRight(tokenColumnWidth)} | {"Embedding (first 3)".PadRight(embeddingColumnWidth)} |";
    }

    private string TruncateText(string text, int maxLength)
    {
        return text.Length > maxLength ? text.Substring(0, maxLength) + Constants.TextElipsis : text;
    }

    private string FormatEmbeddings(List<double> vector, int numEmbeddingsToShow)
    {
        return string.Join(Constants.VectorSeparator, vector.Take(numEmbeddingsToShow)) + (vector.Count > numEmbeddingsToShow ? Constants.TextElipsis : "");
    }

    private string FormatTableRow(KeyValuePair<string, Embedding> entry, int idColumnWidth, int typeColumnWidth, int tokenColumnWidth, int embeddingColumnWidth, int numEmbeddingsToShow, int maxLinesPerRecord, int maxTokenLength)
    {
        var embedding = entry.Value;
        string tokenDisplay = TruncateText(entry.Key, maxTokenLength);
        string firstEmbeddings = FormatEmbeddings(embedding.Vector, numEmbeddingsToShow);

        List<string> idLines = StringUtilities.WrapText(embedding.Id.ToString(), idColumnWidth, maxLinesPerRecord);
        List<string> typeLines = StringUtilities.WrapText(embedding.Type.ToString(), typeColumnWidth, maxLinesPerRecord);
        List<string> tokenLines = StringUtilities.WrapText(tokenDisplay, tokenColumnWidth, maxLinesPerRecord);
        List<string> embeddingLines = StringUtilities.WrapText(firstEmbeddings, embeddingColumnWidth, maxLinesPerRecord);

        return FormatRecord(idLines, typeLines, tokenLines, embeddingLines, idColumnWidth, typeColumnWidth, tokenColumnWidth, embeddingColumnWidth);
    }

    private string FormatRecord(List<string> idLines, List<string> typeLines, List<string> tokenLines, List<string> embeddingLines, int idColumnWidth, int typeColumnWidth, int tokenColumnWidth, int embeddingColumnWidth)
    {
        var result = new StringBuilder();
        for (int i = 0; i < idLines.Count; i++)
        {
            string idLine = i < idLines.Count ? idLines[i] : "";
            string typeLine = i < typeLines.Count ? typeLines[i] : "";
            string tokenLine = i < tokenLines.Count ? tokenLines[i] : "";
            string embeddingLine = i < embeddingLines.Count ? embeddingLines[i] : "";
            result.AppendLine($"| {idLine.PadRight(idColumnWidth)} | {typeLine.PadRight(typeColumnWidth)} | {tokenLine.PadRight(tokenColumnWidth)} | {embeddingLine.PadRight(embeddingColumnWidth)} |");
        }
        result.Append($"| {"".PadRight(idColumnWidth)} | {"".PadRight(typeColumnWidth)} | {"".PadRight(tokenColumnWidth)} | {"".PadRight(embeddingColumnWidth)} |");
        return result.ToString();
    }
}
