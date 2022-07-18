namespace Library.Agent.Action
{
    public abstract class AgentAction
    {
        public abstract AgentActionDefinition GetAgentActionDefinition();

        public abstract void Process();
    }
}
