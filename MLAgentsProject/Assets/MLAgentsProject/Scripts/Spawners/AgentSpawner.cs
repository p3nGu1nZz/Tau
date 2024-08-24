using UnityEngine;
using System.Collections.Generic;

public class AgentSpawner : MonoBehaviour
{
    public GameObject agentPrefab;
    public int numberOfAgents = 20;
    public Vector3 spawnAreaSize = new Vector3(100, 0, 100);
    public CameraFollow cameraFollow;
    public Transform agentsParent;
    public float minimumSpawnDistance = 4f;
    public int maxRetries = 10;
    public WanderingAgentSettings settingsTemplate;
    public WanderingAgentStats statsTemplate;

    private List<GameObject> spawnedAgents = new List<GameObject>();
    private List<Vector3> spawnedObstaclePositions = new List<Vector3>();
    private List<Vector3> spawnedTargetPositions = new List<Vector3>();
    private GameObject activeAgent;

    void Start()
    {
        ObstacleSpawner obstacleSpawner = FindFirstObjectByType<ObstacleSpawner>();
        if (obstacleSpawner != null)
        {
            spawnedObstaclePositions = obstacleSpawner.GetSpawnedPositions();
        }

        TargetSpawner targetSpawner = FindFirstObjectByType<TargetSpawner>();
        if (targetSpawner != null)
        {
            spawnedTargetPositions = targetSpawner.GetSpawnedPositions();
        }

        SpawnAgents();
    }

    void SpawnAgents()
    {
        for (int i = 0; i < numberOfAgents; i++)
        {
            Vector3 spawnPosition;
            int attempts = 0;
            do
            {
                spawnPosition = new Vector3(
                    Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                    0,
                    Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
                );
                attempts++;
            } while (!IsPositionValid(spawnPosition) && attempts < maxRetries);

            if (attempts < maxRetries)
            {
                // Generate a random rotation
                Quaternion spawnRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                GameObject agent = Instantiate(agentPrefab, spawnPosition, spawnRotation, agentsParent);
                agent.transform.position = spawnPosition;
                agent.transform.rotation = spawnRotation;
                WanderingAgent wanderingAgent = agent.GetComponent<WanderingAgent>();
                wanderingAgent.settings = Instantiate(settingsTemplate); // Create a new instance of settings
                wanderingAgent.stats = Instantiate(statsTemplate); // Create a new instance of stats
                spawnedAgents.Add(agent);

                Debug.Log($"Spawned Agent at Position: {spawnPosition} with Rotation: {spawnRotation.eulerAngles}");

                if (i == 0)
                {
                    SetActiveAgent(agent);
                }
            }
            else
            {
                Debug.LogWarning("Failed to find a valid spawn position for an agent after maximum retries.");
            }
        }
    }


    bool IsPositionValid(Vector3 position)
    {
        foreach (GameObject agent in spawnedAgents)
        {
            if (Vector3.Distance(position, agent.transform.position) < minimumSpawnDistance)
            {
                return false;
            }
        }

        foreach (Vector3 spawnedPosition in spawnedObstaclePositions)
        {
            if (Vector3.Distance(position, spawnedPosition) < minimumSpawnDistance)
            {
                return false;
            }
        }

        foreach (Vector3 spawnedPosition in spawnedTargetPositions)
        {
            if (Vector3.Distance(position, spawnedPosition) < minimumSpawnDistance)
            {
                return false;
            }
        }

        return true;
    }

    public void SetActiveAgent(GameObject agent)
    {
        activeAgent = agent;
        if (cameraFollow != null)
        {
            WanderingAgent wanderingAgent = agent.GetComponent<WanderingAgent>();
            cameraFollow.target = activeAgent.transform;
            cameraFollow.rewardDisplayScript.SetAgent(wanderingAgent);
            cameraFollow.rewardDisplayScript.SetHeadingController(wanderingAgent.heading);
        }
    }

    public GameObject GetActiveAgent()
    {
        return activeAgent;
    }
}
