using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ResultFormatter
{
    public string FormatFindResults(List<(string TableName, Embedding Embedding)> results)
    {
        const int numEmbeddingsToShow = 3;
        const int maxLinesPerRecord = 2;
        const int maxTokenLength = 30;

        int idColumnWidth = FinderUtilities.CalculateColumnWidth("Id", results.Select(r => r.Embedding?.Id.ToString() ?? ""), 10);
        int typeColumnWidth = FinderUtilities.CalculateColumnWidth("Type", results.Select(r => r.Embedding?.Type.ToString() ?? ""), 10);
        int tokenColumnWidth = FinderUtilities.CalculateColumnWidth("Token", results.Select(r => r.Embedding?.Token ?? ""), 30, maxTokenLength);
        int tableColumnWidth = FinderUtilities.CalculateColumnWidth("Table", results.Select(r => r.TableName), 15);
        int embeddingColumnWidth = Constants.MaxTableWidth - (idColumnWidth + typeColumnWidth + tokenColumnWidth + tableColumnWidth + 10);

        string separator = new string(Constants.TableSeparator, Constants.MaxTableWidth + 3);
        var result = new StringBuilder();
        result.AppendLine(separator);
        result.AppendLine(FinderUtilities.FormatHeader(idColumnWidth, typeColumnWidth, tokenColumnWidth, tableColumnWidth, embeddingColumnWidth));
        result.AppendLine(separator);

        if (results.Count == 0)
        {
            result.AppendLine($"| {"Token not found.".PadRight(idColumnWidth + typeColumnWidth + tokenColumnWidth + tableColumnWidth + embeddingColumnWidth + 9)} |");
            result.AppendLine(separator);
            return result.ToString();
        }

        foreach (var (tableName, embedding) in results)
        {
            string tokenDisplay = FinderUtilities.TruncateText(embedding.Token, maxTokenLength);
            string firstEmbeddings = FinderUtilities.FormatEmbeddings(embedding.Vector, numEmbeddingsToShow);

            List<string> idLines = StringUtilities.WrapText(embedding.Id.ToString(), idColumnWidth, maxLinesPerRecord);
            List<string> typeLines = StringUtilities.WrapText(embedding.Type.ToString(), typeColumnWidth, maxLinesPerRecord);
            List<string> tokenLines = StringUtilities.WrapText(tokenDisplay, tokenColumnWidth, maxLinesPerRecord);
            List<string> tableLines = StringUtilities.WrapText(tableName, tableColumnWidth, maxLinesPerRecord);
            List<string> embeddingLines = StringUtilities.WrapText(firstEmbeddings, embeddingColumnWidth, maxLinesPerRecord);

            result.AppendLine(FinderUtilities.FormatRecord(idLines, typeLines, tokenLines, tableLines, embeddingLines, idColumnWidth, typeColumnWidth, tokenColumnWidth, tableColumnWidth, embeddingColumnWidth));
            result.AppendLine(separator);
        }

        return result.ToString();
    }
}
