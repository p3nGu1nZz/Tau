using System;

public class IncrementalReward : BaseReward<double[]>
{
    private int columnsToUse;

    public IncrementalReward(int initialColumnsToUse)
    {
        if (initialColumnsToUse > 0 && initialColumnsToUse <= DatabaseConstants.VectorSize)
        {
            this.columnsToUse = initialColumnsToUse;
        }
        else
        {
            throw new ArgumentException($"Invalid number of columns: {initialColumnsToUse}. Must be between 1 and {DatabaseConstants.VectorSize}.");
        }
    }

    public override float Calculate(double[] embedding, double[] expectedEmbedding)
    {
        float totalReward = 0.0f;

        for (int i = 0; i < columnsToUse; i++)
        {
            float difference = (float)Math.Abs(expectedEmbedding[i] - embedding[i]);
            float reward = 1.0f / (1.0f + (float)Math.Exp(-difference)); // Sigmoid function
            totalReward += reward * 2.0f - 1.0f; // Map to range [-1, 1]
        }

        float meanReward = totalReward / columnsToUse;
        return meanReward;
    }

    public void SetColumnsToUse(int columns)
    {
        if (columns > 0 && columns <= DatabaseConstants.VectorSize)
        {
            columnsToUse = columns;
            Log.Message($"Columns to use set to: {columns}");
        }
        else
        {
            throw new ArgumentException($"Invalid number of columns: {columns}. Must be between 1 and {DatabaseConstants.VectorSize}.");
        }
    }
}