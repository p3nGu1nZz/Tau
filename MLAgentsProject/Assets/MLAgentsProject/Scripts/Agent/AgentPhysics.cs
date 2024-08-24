using UnityEngine;
using System.Collections;

public class AgentPhysics : MonoBehaviour
{
    private Rigidbody rb;
    private float moveForce;
    private float turnForce;
    private float maxVelocity;
    private float maxRotVelocity;
    private const float minForce = 0.01f;
    private WanderingAgent agent;

    public void Initialize(WanderingAgent agent, Rigidbody rb, float moveForce, float turnForce, float maxVelocity, float maxRotVelocity)
    {
        this.agent = agent;
        this.rb = rb;
        this.moveForce = moveForce;
        this.turnForce = turnForce;
        this.maxVelocity = maxVelocity;
        this.maxRotVelocity = maxRotVelocity;
    }

    public void ClearMinimumVelocity()
    {
        if (rb.linearVelocity.magnitude < minForce) rb.linearVelocity = Vector3.zero;
        if (Mathf.Abs(rb.angularVelocity.y) < minForce) rb.angularVelocity = Vector3.zero;
    }

    public void CalculateMovement(float moveInputForward, float moveInputStrafe)
    {
        Vector3 moveDirection = transform.forward * moveInputForward * moveForce * Time.fixedDeltaTime;
        Vector3 strafeDirection = transform.right * moveInputStrafe * moveForce * Time.fixedDeltaTime;
        rb.AddForce(moveDirection + strafeDirection, ForceMode.Impulse);

        if (rb.linearVelocity.magnitude > maxVelocity)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxVelocity;
        }

        float distance = rb.linearVelocity.magnitude * Time.fixedDeltaTime;
        agent.stats.distanceTravelled += distance;
    }

    public void CalculateTorque(float turnInput)
    {
        float turn = turnInput * turnForce * 12 * Time.fixedDeltaTime;
        Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, turn, 0));

        float angle = Quaternion.Angle(rb.rotation, rb.rotation * deltaRotation);
        if (angle > agent.settings.maxRotationAngle)
        {
            turn = agent.settings.maxRotationAngle * Mathf.Sign(turnInput) * Time.fixedDeltaTime;
            deltaRotation = Quaternion.Euler(new Vector3(0, turn, 0));
        }

        rb.MoveRotation(rb.rotation * deltaRotation);
        rb.angularVelocity = Vector3.ClampMagnitude(rb.angularVelocity, maxRotVelocity);
    }

    public bool IsIdle()
    {
        return rb.linearVelocity.magnitude < minForce && Mathf.Abs(rb.angularVelocity.y) < minForce;
    }

    public bool HasMovementInput(float moveInputForward, float moveInputStrafe, float turnInput)
    {
        return Mathf.Abs(moveInputForward) > minForce || Mathf.Abs(moveInputStrafe) > minForce || Mathf.Abs(turnInput) > minForce;
    }

    public void ResetVelocityPosition()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        agent.transform.position = Vector3.zero;
        agent.transform.rotation = Quaternion.identity;
    }

    private void HandleCollisionOrTriggerStay(string tag)
    {
        if (agent.settings.positiveTags.Contains(tag))
        {
            agent.AddReward(agent.settings.positiveContactReward);
        }
        else if (agent.settings.negativeTags.Contains(tag))
        {
            agent.AddReward(agent.settings.collisionStayReward);
        }

        agent.stats.UpdateTouchingFlags(tag, true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        string collidedTag = collision.gameObject.tag;

        if (agent.settings.positiveTags.Contains(collidedTag))
        {
            agent.AddReward(agent.settings.positiveReward);
        }
        else if (agent.settings.negativeTags.Contains(collidedTag))
        {
            agent.AddReward(agent.settings.negativeReward);
        }

        agent.stats.UpdateTouchingFlags(collidedTag, true);
    }

    private void OnCollisionExit(Collision collision)
    {
        string collidedTag = collision.gameObject.tag;

        agent.StartCoroutine(UpdateTouchingFlagsWithDelay(collidedTag, false));
    }

    private void OnTriggerEnter(Collider other)
    {
        string collidedTag = other.gameObject.tag;

        if (agent.settings.positiveTags.Contains(collidedTag))
        {
            agent.stats.isTouchingTarget = true;
        }
        else if (agent.settings.negativeTags.Contains(collidedTag))
        {
            agent.AddReward(agent.settings.negativeReward);
        }

        agent.stats.UpdateTouchingFlags(collidedTag, true);
    }

    private void OnTriggerExit(Collider other)
    {
        string collidedTag = other.gameObject.tag;

        agent.StartCoroutine(UpdateTouchingFlagsWithDelay(collidedTag, false));
    }

    private void OnCollisionStay(Collision collision)
    {
        HandleCollisionOrTriggerStay(collision.gameObject.tag);
    }

    private void OnTriggerStay(Collider other)
    {
        HandleCollisionOrTriggerStay(other.gameObject.tag);
    }

    private IEnumerator UpdateTouchingFlagsWithDelay(string tag, bool isTouching)
    {
        yield return new WaitForSeconds(agent.settings.touchingFlagDelay);
        agent.stats.UpdateTouchingFlags(tag, isTouching);
    }
}
