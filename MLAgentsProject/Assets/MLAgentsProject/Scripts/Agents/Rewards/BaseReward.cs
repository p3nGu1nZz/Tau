public abstract class BaseReward<T>
{
    public abstract float CalculateReward(T embedding, T expectedEmbedding);
}
