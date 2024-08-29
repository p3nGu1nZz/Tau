using UnityEngine;

public abstract class AgentDelegator<T, U> : MonoBehaviour where T : MonoBehaviour where U : MonoBehaviour, IBaseAgent
{
    private static T _instance;
    public static T Instance => _instance;

    public U Agent;
    public AgentData Data;
    public BaseReward<double[]> Reward;
    public bool IsInitialized = false;
    public bool IsProcessing = false;

    protected virtual void Awake()
    {
        _instance = this as T;
        Data = new AgentData();
    }

    protected virtual void Start()
    {
        Initialize();
    }

    protected abstract void Initialize();
    protected abstract void Setup();
}
