using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections.Generic;
using System.Linq;
using System;

public abstract class BaseAgent : Agent, IBaseAgent
{
    public AgentData Data { get; private set; }
    public BaseReward<double[]> RewardCalculator { get; set; }

    public void Setup()
    {
        Data = new AgentData();
        InitializeVocabulary();
    }

    public override void Initialize()
    {
        try
        {
            if (Data.Vocabulary != null)
            {
                LogTokens();
            }
            else
            {
                throw new InvalidOperationException("Vocabulary is not initialized.");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Error during initialization: {ex.Message}");
        }
    }

    public override void OnEpisodeBegin()
    {
        Log.Message("New episode has begun.");
        ResetAgent();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        try
        {
            if (Data.CachedInputVector != null)
            {
                AgentUtilities.AddObservations(sensor, Data.CachedInputVector);
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Error during observation collection: {ex.Message}");
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        try
        {
            if (!CheckActionLength(actions.ContinuousActions.Length))
            {
                throw new ArgumentException($"Expected 384 continuous actions, but received {actions.ContinuousActions.Length}.");
            }

            ProcessActionToVector(actions.ContinuousActions);
            CalculateReward();
        }
        catch (Exception ex)
        {
            Log.Error($"Error during action reception: {ex.Message}");
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut) { }

    public void UpdateWithReward(float reward)
    {
        SetReward(reward);
    }

    protected void InitializeVocabulary()
    {
        try
        {
            Data.Vocabulary = GetVocabulary();
            Log.Message($"Loaded {Data.Vocabulary.Count} tokens into agent's vocabulary.");
        }
        catch (Exception ex)
        {
            Log.Error($"Error during vocabulary initialization: {ex.Message}");
        }
    }

    protected Dictionary<string, Embedding> GetVocabulary()
    {
        return Database.Instance.GetTable(TableNames.Vocabulary);
    }

    protected void LogTokens()
    {
        foreach (var token in Data.Vocabulary.Keys)
        {
            Log.Message($"Token: {token}");
        }
    }

    protected bool CheckActionLength(int length)
    {
        if (length != DatabaseConstants.VectorSize)
        {
            throw new ArgumentException($"Expected 384 continuous actions, but received {length}.");
        }
        return true;
    }

    protected void ProcessActionToVector(ActionSegment<float> actions)
    {
        Data.CachedOutputVector = AgentUtilities.ConvertActionsToDouble(actions);
        Data.OutputVector = AgentUtilities.ConvertToFloatArray(Data.CachedOutputVector);
    }

    protected abstract void CalculateReward();

    public void ResetAgent()
    {
        Data.CachedInputVector = null;
        Data.CachedOutputVector = null;
        Data.ExpectedOutputVector = null;
        Data.InputVector = null;
        Data.OutputVector = null;
    }
}
