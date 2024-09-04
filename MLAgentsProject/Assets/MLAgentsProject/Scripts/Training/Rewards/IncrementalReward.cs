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
        float threshold = 0.5f;
        float totalReward = 0.0f;

        //Log.Message($"Calculating reward using the first {columnsToUse} columns.");
        //Log.Message($"V: {embedding[0]}..., E: {expectedEmbedding[0]}..., D: {Math.Abs(expectedEmbedding[0] - embedding[0])}");

        for (int i = 0; i < columnsToUse; i++)
        {
            if (Math.Abs(expectedEmbedding[i] - embedding[i]) < threshold)
            {
                totalReward += 1.0f;
            }
            else
            {
                totalReward -= 1.0f;
            }
        }

        float meanReward = totalReward / columnsToUse;
        //Log.Message($"Total reward: {totalReward}, Mean reward: {meanReward}");
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
