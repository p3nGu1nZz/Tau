using System;
using System.IO;
using System.Threading.Tasks;

public class TokenOptimizerTask
{
    public static async Task Execute(string inputFile)
    {
        try
        {
            Log.Message($"Starting optimization for input file: {inputFile}");
            await Optimizer.Instance.Optimize(inputFile);
        }
        catch (Exception ex)
        {
            Log.Error($"Exception occurred during optimization: {ex.Message}");
            throw;
        }
    }
}
