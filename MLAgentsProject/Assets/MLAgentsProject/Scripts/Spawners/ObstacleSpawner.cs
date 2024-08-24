using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public int numberOfObstacles = 20;
    public Vector3 spawnAreaSize = new Vector3(100, 0, 100);
    public Transform obstaclesParent;
    public float minimumSpawnDistance = 4f;
    public int maxRetries = 10;

    private List<Vector3> spawnedPositions = new List<Vector3>();

    void Start()
    {
        SpawnObstacles();
    }

    void SpawnObstacles()
    {
        for (int i = 0; i < numberOfObstacles; i++)
        {
            Vector3 spawnPosition;
            int attempts = 0;
            bool validPosition = false;

            do
            {
                spawnPosition = new Vector3(
                    Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                    0, // Ensure y is set to ground level
                    Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
                );
                attempts++;
                validPosition = IsPositionValid(spawnPosition);
            } while (!validPosition && attempts < 100);

            if (validPosition)
            {
                GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity, obstaclesParent);
                spawnedPositions.Add(spawnPosition);
                Debug.Log($"Spawned Obstacle at Position: {spawnPosition}");

                // Check for collisions and reposition if necessary
                int retries = 0;
                while (IsColliding(obstacle) && retries < maxRetries)
                {
                    spawnPosition = new Vector3(
                        Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                        0, // Ensure y is set to ground level
                        Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
                    );
                    Debug.Log($"Spawn collision Retry position {spawnPosition}");
                    obstacle.transform.position = spawnPosition;
                    retries++;
                }

                if (retries >= maxRetries)
                {
                    Debug.LogWarning("Failed to find a non-colliding position for an obstacle after maximum retries. Removing obstacle.");
                    Destroy(obstacle);
                    spawnedPositions.Remove(spawnPosition);
                }
            }
            else
            {
                Debug.LogWarning("Failed to find a valid spawn position for an obstacle after 100 attempts.");
            }
        }
    }

    bool IsPositionValid(Vector3 position)
    {
        foreach (Vector3 spawnedPosition in spawnedPositions)
        {
            if (Vector3.Distance(position, spawnedPosition) < minimumSpawnDistance)
            {
                return false;
            }
        }
        return true;
    }

    bool IsColliding(GameObject obstacle)
    {
        Collider[] colliders = Physics.OverlapSphere(obstacle.transform.position, minimumSpawnDistance);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Obstacle") && collider.gameObject != obstacle)
            {
                return true;
            }
        }
        return false;
    }

    public List<Vector3> GetSpawnedPositions()
    {
        return spawnedPositions;
    }
}
