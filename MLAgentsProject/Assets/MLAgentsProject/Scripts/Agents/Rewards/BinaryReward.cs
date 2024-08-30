using System;

public class BinaryReward : BaseReward<double[]>
{
    public override float Calculate(double[] embedding, double[] expectedEmbedding)
    {
        Log.Message($"Calculate mean binary reward. vectorSize={embedding.Length}");
        float threshold = 0.1f;
        float totalReward = 0.0f;

        for (int i = 0; i < embedding.Length; i++)
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

        float meanReward = totalReward / embedding.Length;
        Log.Message($"Calculated mean reward: {meanReward}");
        return meanReward;
    }
}
