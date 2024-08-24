using UnityEngine;

public class AgentHeadingController : MonoBehaviour
{
    public Transform agent;
    private Vector3 headingDirection;
    public LineRenderer lineRenderer;
    public WanderingAgent wanderingAgent;

    void Start()
    {
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        if (wanderingAgent.state.GetCurrentState() == "WanderState")
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, agent.position);
            lineRenderer.SetPosition(1, agent.position + headingDirection * 3);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    public Vector3 GetHeadingDirection()
    {
        return headingDirection;
    }

    public void SetHeadingDirection(Vector3 newHeadingDirection)
    {
        headingDirection = newHeadingDirection;
    }

    public Vector3 GetRandomHeading()
    {
        float randomAngle = Random.Range(0f, 360f);
        Vector3 randomHeading = new Vector3(Mathf.Cos(randomAngle * Mathf.Deg2Rad), 0, Mathf.Sin(randomAngle * Mathf.Deg2Rad));
        return randomHeading;
    }
}
