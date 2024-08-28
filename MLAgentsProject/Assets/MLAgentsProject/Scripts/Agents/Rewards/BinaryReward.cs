using System;

public class BinaryReward : BaseReward<double[]>
{
    public override double CalculateReward(double[] embedding, double[] expectedEmbedding)
    {
        double threshold = 0.1;
        bool isWithinThreshold = true;

        for (int i = 0; i < embedding.Length; i++)
        {
            if (Math.Abs(expectedEmbedding[i] - embedding[i]) >= threshold)
            {
                isWithinThreshold = false;
                break;
            }
        }

        return isWithinThreshold ? 1.0 : -1.0;
    }
}
