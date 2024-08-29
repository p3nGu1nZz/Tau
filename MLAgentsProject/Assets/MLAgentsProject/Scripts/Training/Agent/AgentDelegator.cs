using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.MLAgents.Policies;
using System;

public abstract class AgentDelegator<T, U> : MonoBehaviour where T : MonoBehaviour where U : MonoBehaviour, IBaseAgent
{
    private static T _instance;
    public static T Instance => _instance;

    public U agent;
    public AgentData data;
    public BaseReward<double[]> rewardCalculator;
    public bool isInitialized = false;
    public bool isProcessing = false;

    protected virtual void Awake()
    {
        _instance = this as T;
        data = new AgentData();
    }

    protected virtual void Start()
    {
        Initialize();
    }

    protected abstract void Initialize();
    protected abstract void SetupAgent();
}
