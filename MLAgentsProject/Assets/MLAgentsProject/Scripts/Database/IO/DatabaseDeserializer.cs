using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class DatabaseDeserializer
{
    public static EmbeddingsList Deserialize(string customFormat)
    {
        Log.Message("Starting deserialization of custom format.");

        // Split the content into lines
        var lines = customFormat.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        Log.Message($"Total lines: {lines.Count}");

        // Remove empty or null lines
        lines = lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToList();
        Log.Message($"Non-empty lines: {lines.Count}");

        var embeddingsList = new EmbeddingsList
        {
            tables = new Dictionary<string, List<Embedding>>()
        };

        embeddingsList.version = ExtractField(lines, DatabaseTokens.Version);
        embeddingsList.model_name = ExtractField(lines, DatabaseTokens.Model_Name);
        embeddingsList.organization = ExtractField(lines, DatabaseTokens.Organization);
        embeddingsList.total_embeddings = int.Parse(ExtractField(lines, DatabaseTokens.Total_Embeddings));

        // Extract tables
        embeddingsList.tables = ExtractTables(lines);

        Log.Message("Deserialization complete.");
        return embeddingsList;
    }

    private static string ExtractField(List<string> lines, DatabaseTokens token)
    {
        string tokenString = DatabaseStrings.Tokens[token];
        //Log.Message($"Extracting field: {tokenString}");

        int i = 0;
        while (i < lines.Count)
        {
            if (lines[i].Contains(tokenString))
            {
                //Log.Message($"Found field {tokenString} at line {i}");
                return ExtractValue(lines, i + 1);
            }
            i++;
        }
        throw new FormatException($"Field {tokenString} not found in content.");
    }

    private static string ExtractValue(List<string> lines, int startIndex)
    {
        //Log.Message($"Extracting value starting at line {startIndex}");
        if (startIndex < lines.Count)
        {
            string value = lines[startIndex].Trim();
            if (!value.StartsWith("<|"))
            {
                //Log.Message($"Extracted value: {value}");
                return value;
            }
        }
        Log.Error($"Expected value at line {startIndex}, but found none. Context: {string.Join(" ", lines.Skip(Math.Max(0, startIndex - 2)).Take(4))}");
        throw new FormatException($"Expected value at line {startIndex}, but found none.");
    }

    private static Dictionary<string, List<Embedding>> ExtractTables(List<string> lines)
    {
        Log.Message("Extracting tables.");
        var tables = new Dictionary<string, List<Embedding>>();
        int i = 0;
        while (i < lines.Count)
        {
            Log.Message($"Parsing line {i}: {StringUtilities.TruncateLogMessage(lines[i])}");
            if (lines[i].Contains(DatabaseStrings.Tokens[DatabaseTokens.Table]))
            {
                string tableName = ExtractValue(lines, i + 1);
                Log.Message($"Found table: {StringUtilities.TruncateLogMessage(tableName)}");
                tables[tableName] = new List<Embedding>();

                // Skip to the next table or end token
                i += 2; // Move to the next line after the table name
                while (i < lines.Count && !lines[i].Contains(DatabaseStrings.Tokens[DatabaseTokens.Table]) && !lines[i].Contains(DatabaseStrings.Tokens[DatabaseTokens.End]))
                {
                    //Log.Message($"Parsing line {i}: {StringUtilities.TruncateLogMessage(lines[i])}");
                    if (lines[i].Contains(DatabaseStrings.Tokens[DatabaseTokens.Embedding]))
                    {
                        //Log.Message("Found embedding start.");
                        var embedding = ExtractEmbedding(lines, ref i);
                        tables[tableName].Add(embedding);
                    }
                    i++;
                }
            }
            i++;
        }
        return tables;
    }

    private static Embedding ExtractEmbedding(List<string> lines, ref int index)
    {
        //Log.Message($"Extracting embedding at index {index} with total lines count {lines.Count}.");
        int id = 0;
        string token = null;
        EmbeddingType type = default;
        List<double> vector = null;

        try
        {
            while (index < lines.Count)
            {
                string line = lines[index].Trim();
                //Log.Message($"Processing line: {line}");

                if (line == DatabaseStrings.Tokens[DatabaseTokens.Embedding])
                {
                    //Log.Message("Found embedding start.");
                    index++;
                }
                else if (line == DatabaseStrings.Tokens[DatabaseTokens.Id])
                {
                    //Log.Message("Found Id token.");
                    id = int.Parse(lines[++index].Trim());
                    //Log.Message($"Extracted Id: {id}");
                    index++;
                }
                else if (line == DatabaseStrings.Tokens[DatabaseTokens.Token])
                {
                    //Log.Message("Found Token token.");
                    token = lines[++index].Trim();
                    //Log.Message($"Extracted Token: {token}");
                    index++;
                }
                else if (line == DatabaseStrings.Tokens[DatabaseTokens.Type])
                {
                    //Log.Message("Found Type token.");
                    type = (EmbeddingType)Enum.Parse(typeof(EmbeddingType), lines[++index].Trim());
                    //Log.Message($"Extracted Type: {type}");
                    index++;
                }
                else if (line == DatabaseStrings.Tokens[DatabaseTokens.Vector])
                {
                    //Log.Message("Found Vector token.");
                    vector = ExtractVector(lines, ++index, '|');
                    //Log.Message($"Extracted Vector with {vector.Count} elements.");
                    index++;
                }
                else if (line == DatabaseStrings.Tokens[DatabaseTokens.End])
                {
                    //Log.Message("Found end token.");
                    index++;
                    if (id != 0 && token != null && vector != null)
                    {
                        break;
                    }
                }
                else
                {
                    index++;
                }
            }

            if (token == null || vector == null)
            {
                throw new FormatException("Failed to extract embedding: one or more fields are null.");
            }

            return new Embedding(id, token, vector.ToArray(), type);
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to extract embedding: {ex.Message}");
            throw;
        }
    }

    private static List<double> ExtractVector(List<string> lines, int startIndex, char delimiter)
    {
        //Log.Message($"Extracting vector starting at line {startIndex}");
        var vector = new List<double>();

        while (startIndex < lines.Count && !lines[startIndex].StartsWith("<|"))
        {
            //Log.Message($"Parsing vector line: {StringUtilities.TruncateLogMessage(lines[startIndex])}");
            try
            {
                // Remove square brackets and split by delimiter
                string line = lines[startIndex].Trim().Trim('[', ']');
                vector.AddRange(line.Split(delimiter).Select(double.Parse));
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to parse vector line: {StringUtilities.TruncateLogMessage(lines[startIndex])}. Error: {ex.Message}");
            }
            startIndex++;
        }

        Log.Message($"Extracted vector with {vector.Count} elements.");
        return vector;
    }
}
