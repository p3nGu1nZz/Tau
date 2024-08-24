public class AgentStateBase
{
    protected WanderingAgent agent;

    public AgentStateBase(WanderingAgent agent)
    {
        this.agent = agent;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void LateUpdate() { }
    public virtual void Exit() { }
}
