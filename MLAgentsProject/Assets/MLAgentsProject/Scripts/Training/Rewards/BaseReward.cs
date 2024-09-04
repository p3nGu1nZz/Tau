public abstract class BaseReward<T>
{
    public abstract float Calculate(T output, T expected);
}
