public abstract class BaseReward<T>
{
    public abstract double CalculateReward(T embedding, T expectedEmbedding);
}
