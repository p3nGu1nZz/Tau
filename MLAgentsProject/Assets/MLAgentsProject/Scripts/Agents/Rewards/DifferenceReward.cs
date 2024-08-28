using System;

public class DifferenceReward : BaseReward<double[]>
{
    public override double CalculateReward(double[] embedding, double[] expectedEmbedding)
    {
        double totalDifference = 0.0;

        for (int i = 0; i < embedding.Length; i++)
        {
            totalDifference += Math.Abs(expectedEmbedding[i] - embedding[i]);
        }

        double averageDifference = totalDifference / embedding.Length;
        return -averageDifference;
    }
}
