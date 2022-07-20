using Library.Agent.Action;

namespace GuacamoleAgent.Agent.Action
{
    public class ConsoleLogAgentAction : AgentAction
    {
        public override AgentActionDefinition GetAgentActionDefinition()
        {
            AgentActionDefinition agentActionDefinition = new AgentActionDefinition();
            agentActionDefinition.DisplayName = "Console log on agent";
            return agentActionDefinition;
        }

        public override void Process()
        {
            Console.WriteLine("console log");
        }
    }
}
