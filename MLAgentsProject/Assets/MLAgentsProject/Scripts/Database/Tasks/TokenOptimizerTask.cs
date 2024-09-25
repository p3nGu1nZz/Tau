using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class TokenOptimizerTask
{
    public static async Task Execute(string inputFile)
    {
        try
        {
            Log.Message($"Starting optimization for input file: {inputFile}");

            string output = await Optimizer.Instance.Optimize(inputFile);
            Log.Message($"Optimizer output: {output}");

            string pattern = @"Reduced embeddings saved to (\S+)";
            Match match = Regex.Match(output, pattern);
            if (match.Success)
            {
                string outputFile = match.Groups[1].Value;
                if (!File.Exists(outputFile))
                {
                    Log.Error($"Output file '{outputFile}' not found.");
                    throw new FileNotFoundException($"Output file '{outputFile}' not found.");
                }
            }
            else
            {
                Log.Error("Output file name not found in the optimizer output.");
                throw new InvalidOperationException("Output file name not found in the optimizer output.");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Exception occurred during optimization: {ex.Message}");
            throw;
        }
    }
}
