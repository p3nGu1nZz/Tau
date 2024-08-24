using UnityEngine;
using System.Collections.Generic;

public class TargetSpawner : MonoBehaviour
{
    public GameObject targetPrefab;
    public int numberOfTargets = 10;
    public Vector3 spawnAreaSize = new Vector3(200, 0, 200);
    public Transform targetsParent;
    public float minimumSpawnDistance = 4f;
    public int maxRetries = 10;

    private List<Vector3> spawnedTargetPositions = new List<Vector3>();
    private List<Vector3> spawnedObstaclePositions = new List<Vector3>();

    void Start()
    {
        // Get the positions of already spawned obstacles
        ObstacleSpawner obstacleSpawner = FindFirstObjectByType<ObstacleSpawner>();
        if (obstacleSpawner != null)
        {
            spawnedObstaclePositions = obstacleSpawner.GetSpawnedPositions();
        }

        SpawnTargets();
    }

    void SpawnTargets()
    {
        for (int i = 0; i < numberOfTargets; i++)
        {
            Vector3 spawnPosition;
            int attempts = 0;
            do
            {
                spawnPosition = new Vector3(
                    Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                    0, // Ensure y is set to ground level
                    Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
                );
                attempts++;
            } while (!IsPositionValid(spawnPosition) && attempts < maxRetries);

            if (attempts < maxRetries)
            {
                Instantiate(targetPrefab, spawnPosition, Quaternion.identity, targetsParent);
                spawnedTargetPositions.Add(spawnPosition);
            }
            else
            {
                Debug.LogWarning("Failed to find a valid spawn position for a target after maximum retries.");
            }
        }
    }

    bool IsPositionValid(Vector3 position)
    {
        foreach (Vector3 spawnedPosition in spawnedTargetPositions)
        {
            if (Vector3.Distance(position, spawnedPosition) < minimumSpawnDistance)
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

        return true;
    }

    public List<Vector3> GetSpawnedPositions()
    {
        return spawnedTargetPositions;
    }
}
