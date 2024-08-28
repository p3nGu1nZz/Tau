using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections.Generic;
using System.Linq;
using System;

public class TauAgent : Agent
{
    public Dictionary<string, Embedding> Vocabulary { get; private set; }
    private double[] input;
    private double[] output;

    public void Setup()
    {
        InitializeVocabulary();
    }

    public override void Initialize()
    {
        try
        {
            if (Vocabulary != null)
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
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        try
        {
            if (input != null)
            {
                AddObservations(sensor, input);
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
            if (!CheckLength(actions.ContinuousActions.Length))
            {
                throw new ArgumentException($"Expected 384 continuous actions, but received {actions.ContinuousActions.Length}.");
            }

            ProcessActions(actions.ContinuousActions);
        }
        catch (Exception ex)
        {
            Log.Error($"Error during action reception: {ex.Message}");
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut) { }

    public void SetObservations(double[] observations)
    {
        this.input = observations;
    }

    public double[] GetOutputEmbedding()
    {
        return output;
    }

    public void UpdateWithReward(double reward)
    {
        AddReward((float)reward);
    }

    private void InitializeVocabulary()
    {
        try
        {
            Vocabulary = GetVocabulary();
            Log.Message($"Loaded {Vocabulary.Count} tokens into agent's vocabulary.");
        }
        catch (Exception ex)
        {
            Log.Error($"Error during vocabulary initialization: {ex.Message}");
        }
    }

    private Dictionary<string, Embedding> GetVocabulary()
    {
        return Database.Instance.GetTable(TableNames.Vocabulary);
    }

    private void LogTokens()
    {
        foreach (var token in Vocabulary.Keys)
        {
            Log.Message($"Token: {token}");
        }
    }

    private bool CheckLength(int length)
    {
        if (length != DatabaseConstants.VectorSize)
        {
            throw new ArgumentException($"Expected 384 continuous actions, but received {length}.");
        }
        return true;
    }

    private float[] GetFloatVector(double[] vector)
    {
        return vector.Select(d => (float)d).ToArray();
    }

    private void ProcessActions(ActionSegment<float> actions)
    {
        output = ConvertActionsToDouble(actions);
    }

    private double[] ConvertActionsToDouble(ActionSegment<float> actions)
    {
        double[] result = new double[actions.Length];
        for (int i = 0; i < actions.Length; i++)
        {
            result[i] = actions[i];
        }
        return result;
    }

    private void AddObservations(VectorSensor sensor, double[] observations)
    {
        float[] floatObservations = GetFloatVector(observations);
        foreach (var value in floatObservations)
        {
            sensor.AddObservation(value);
        }
    }
}
