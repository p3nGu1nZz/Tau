using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections.Generic;
using System;
using System.Linq;

public abstract class BaseAgent : Agent, IBaseAgent
{
    public AgentData Data { get; private set; }
    public float StepReward { get; set; }
    public int EpisodeCount { get; set; }

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
            EpisodeCount = 0;
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
        Log.Message($"New episode {EpisodeCount++} has begun.");
        ResetAgent();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        try
        {
            if (Data.Observations != null && Data.Observations.Length == DatabaseConstants.VectorSize)
            {
                sensor.AddObservation(Data.Observations);
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
            ProcessActions(actions.ContinuousActions);
        }
        catch (Exception ex)
        {
            Log.Error($"Error during action reception: {ex.Message}");
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        try
        {
            Log.Message("Using heuristic to determine actions.");
            var continuousActions = actionsOut.ContinuousActions;
            for (int i = 0; i < continuousActions.Length; i++)
            {
                continuousActions[i] = UnityEngine.Random.Range(-1f, 1f);
            }

            ProcessActions(actionsOut.ContinuousActions);
        }
        catch (Exception ex)
        {
            Log.Error($"Error during heuristic action processing: {ex.Message}");
        }
    }

    public void ProcessActions(ActionSegment<float> continuousActions)
    {
        CheckActionLength(continuousActions.Length);
        Data.ModelOutput = AgentUtilities.ConvertActionsToDouble(continuousActions);
        //Log.Message(StringUtilities.TruncateLogMessage($"process_actions: ModelOutput={StringUtilities.ConvertVectorToString(Data.ModelOutput)}"));

        HandleReward();
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

    protected abstract void HandleReward();

    public void ResetAgent()
    {
        //Log.Message("Resetting agent.");
        Data.ModelInput = null;
        Data.ModelOutput = null;
        Data.ExpectedOutput = null;
        Data.Observations = null;
    }
}
