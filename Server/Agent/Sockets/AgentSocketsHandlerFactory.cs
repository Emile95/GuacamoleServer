using Server.Agent.Action;
using Server.Agent.Request;
using Server.Config;

namespace Server.Agent.Sockets
{
    public static class AgentSocketsHandlerFactory
    {
        public static AgentSocketsHandler CreateAgentSocketsHandler(AgentsSocketConfig agentSocketsConfig, Library.Logger.ILogger logger, AgentManager agentManager, ServerAgentActionManager serverAgentActionManager, RequestReceivedHandler requestReceivedHandler)
        {
            switch(agentSocketsConfig.Protocol)
            {
                case "tcp": return new TCPAgentSocketsHandler(logger, agentSocketsConfig.Port, agentManager, serverAgentActionManager, requestReceivedHandler);
            }
            throw new Exception("most prodive a valid string expression for socket protocol : '" + agentSocketsConfig.Protocol + "' is Invalid");
        }
    }
}
