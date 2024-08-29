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
        Log.Message("Setting up BaseAgent.");
        Data = new AgentData();
        InitializeVocabulary();
    }

    public override void Initialize()
    {
        try
        {
            Log.Message("Initializing BaseAgent.");
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
            Log.Message("Collecting observations.");
            if (Data.ModelInput != null)
            {
                AgentUtilities.AddObservations(sensor, Data.ModelInput);
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
            Log.Message("Action received.");
            if (!CheckActionLength(actions.ContinuousActions.Length))
            {
                throw new ArgumentException($"Expected 384 continuous actions, but received {actions.ContinuousActions.Length}.");
            }

            ProcessActionToVector(actions.ContinuousActions);
            HandleReward();
        }
        catch (Exception ex)
        {
            Log.Error($"Error during action reception: {ex.Message}");
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        Log.Message("Using heuristic to determine actions.");
        var continuousActions = actionsOut.ContinuousActions;
        for (int i = 0; i < continuousActions.Length; i++)
        {
            continuousActions[i] = UnityEngine.Random.Range(-1f, 1f); // Example heuristic: random actions
        }
    }

    public void UpdateWithReward(float reward)
    {
        Log.Message($"Updating with reward: {reward}");
        SetReward(reward);
    }

    protected void InitializeVocabulary()
    {
        try
        {
            Log.Message("Initializing vocabulary.");
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
        Log.Message("Processing action to vector.");
        Data.ModelOutput = AgentUtilities.ConvertActionsToDouble(actions);
    }

    protected abstract void HandleReward();

    public void ResetAgent()
    {
        Log.Message("Resetting agent.");
        Data.ModelInput = null;
        Data.ModelOutput = null;
        Data.ExpectedOutput = null;
        Data.Observations = null;
    }
}
