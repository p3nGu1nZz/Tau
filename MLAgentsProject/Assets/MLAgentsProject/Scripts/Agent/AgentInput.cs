using Unity.MLAgents.Actuators;
using UnityEngine;
using UnityEngine.InputSystem;

public class AgentInput : MonoBehaviour
{
    private WanderingAgent agent;
    private InputAction moveAction;
    private InputAction lookAction;

    public void Initialize(WanderingAgent agent)
    {
        this.agent = agent;
        InitializeInputActions();
    }

    private void InitializeInputActions()
    {
        var playerInput = new InputActionMap("Player");
        moveAction = playerInput.AddAction("Move", binding: "<Gamepad>/leftStick");
        lookAction = playerInput.AddAction("Look", binding: "<Gamepad>/rightStick");
        moveAction.Enable();
        lookAction.Enable();
    }

    public void HandleHeuristic(in ActionBuffers actionsOut)
    {
        if (agent.agentSpawner != null && agent.agentSpawner.GetActiveAgent() == agent.gameObject)
        {
            var continuousActionsOut = actionsOut.ContinuousActions;
            Vector2 moveInputVector = moveAction.ReadValue<Vector2>();
            Vector2 lookInputVector = lookAction.ReadValue<Vector2>();

            continuousActionsOut[0] = moveInputVector.y;
            continuousActionsOut[1] = lookInputVector.x;
            continuousActionsOut[2] = moveInputVector.x;
        }
    }
}
