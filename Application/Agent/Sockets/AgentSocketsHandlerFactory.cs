using Application.Agent.Action;
using Application.Agent.Request;
using Application.Config;

namespace Application.Agent.Sockets
{
    public static class AgentSocketsHandlerFactory
    {
        public static AgentSocketsHandler CreateAgentSocketsHandler(AgentsSocketConfig agentSocketsConfig, Library.Logger.ILogger logger, AgentManager agentManager, ServerAgentActionManager serverAgentActionManager, RequestReceivedHandler requestReceivedHandler)
        {
            switch(agentSocketsConfig.Protocol)
            {
                case "tcp": return new TCPAgentSocketsHandler(logger, agentSocketsConfig.Port, agentManager, serverAgentActionManager, requestReceivedHandler);
            }
            return null;
        }
    }
}
