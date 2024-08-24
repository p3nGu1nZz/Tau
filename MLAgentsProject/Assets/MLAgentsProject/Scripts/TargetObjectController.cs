using UnityEngine;

public class TargetObjectController : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 positionRange = new Vector3(10f, 0f, 10f);
    public float minMoveInterval = 60f;
    public float maxMoveInterval = 120f;

    private float moveCooldown;
    private float nextMoveTime;

    void Start()
    {
        SetNextMoveTime();
        MoveToRandomPosition();
    }

    void Update()
    {
        if (Time.time >= nextMoveTime)
        {
            MoveToRandomPosition();
            SetNextMoveTime();
        }
    }

    private void SetNextMoveTime()
    {
        moveCooldown = Random.Range(minMoveInterval, maxMoveInterval);
        nextMoveTime = Time.time + moveCooldown;
    }

    private void MoveToRandomPosition()
    {
        Vector3 newPosition = new Vector3(
            Random.Range(-positionRange.x, positionRange.x),
            transform.position.y,
            Random.Range(-positionRange.z, positionRange.z)
        );
        transform.position = newPosition;
    }
    private void OnTriggerEnter(Collider other)
    {
        MoveToRandomPosition();
    }
}
