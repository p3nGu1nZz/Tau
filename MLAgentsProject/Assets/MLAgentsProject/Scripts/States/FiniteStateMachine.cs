using System.Collections.Generic;

public class FiniteStateMachine
{
    private AgentStateBase currentState;
    private Dictionary<AgentState, AgentStateBase> states;
    private WanderingAgent _agent;

    public FiniteStateMachine(WanderingAgent agent)
    {
        _agent = agent;
        states = new Dictionary<AgentState, AgentStateBase>();
    }

    public void Initialize(AgentState state)
    {
        states = new Dictionary<AgentState, AgentStateBase>
        {
            { AgentState.Idle, new IdleState(_agent) },
            { AgentState.Wander, new WanderState(_agent) }
        };
        ChangeState(state);
    }

    public void ChangeState(AgentState state)
    {
        currentState?.Exit();
        currentState = states[state];
        currentState.Enter();
    }

    public void Update()
    {
        currentState?.Update();
    }

    public void FixedUpdate()
    {
        currentState?.FixedUpdate();
    }

    public void LateUpdate()
    {
        currentState?.LateUpdate();
    }

    public void ResetState()
    {
        currentState?.Exit();
        currentState = states[AgentState.Idle];
        currentState.Enter();
    }

    public string GetCurrentState()
    {
        return currentState?.GetType().Name;
    }

    public float GetCurrentStateAsFloat()
    {
        switch (GetCurrentState())
        {
            case "IdleState":
                return (float)AgentState.Idle;
            case "WanderState":
                return (float)AgentState.Wander;
            default:
                return -1f;
        }
    }
}
