using UnityEngine;

public static class TrainingUtilities
{
    public static void EnableTauAgent(GameObject tauAgent)
    {
        var tauAgentComponent = tauAgent.GetComponent<TauAgent>();
        if (tauAgentComponent != null)
        {
            tauAgentComponent.enabled = true;
        }
        else
        {
            Log.Error("TauAgent component not found on the instantiated prefab.");
        }
    }

    public static void DisableTauAgent(GameObject tauAgent)
    {
        var tauAgentComponent = tauAgent.GetComponent<TauAgent>();
        if (tauAgentComponent != null)
        {
            tauAgentComponent.enabled = false;
        }
        else
        {
            Log.Error("TauAgent component not found on the instantiated prefab.");
        }
    }
}
