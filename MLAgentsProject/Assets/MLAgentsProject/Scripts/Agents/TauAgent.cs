using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections.Generic;

public class TauAgent : Agent
{
    public Dictionary<string, Embedding> Vocabulary { get; private set; }

    public void Setup()
    {
        InitializeVocabulary();
    }

    public override void Initialize()
    {
        if (Vocabulary != null)
        {
            foreach (var token in Vocabulary.Keys)
            {
                Log.Message($"Token: {token}");
            }
        }
        else
        {
            Log.Error("Vocabulary is not initialized.");
        }
    }

    public override void OnEpisodeBegin() { }

    public override void CollectObservations(VectorSensor sensor) { }

    public override void OnActionReceived(ActionBuffers actions) { }

    public override void Heuristic(in ActionBuffers actionsOut) { }

    private void InitializeVocabulary()
    {
        Vocabulary = Database.Instance.GetTable(TableNames.Vocabulary);
        Log.Message($"Loaded {Vocabulary.Count} tokens into agent's vocabulary.");
    }
}
