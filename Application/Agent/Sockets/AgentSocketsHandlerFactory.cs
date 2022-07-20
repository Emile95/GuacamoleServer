using Application.Agent.Action;
using Application.Agent.Request;
using Application.Config;

namespace Application.Agent.Sockets
{
    public static class AgentSocketsHandlerFactory
    {
        public static AgentSocketsHandler CreateAgentSocketsHandler(AgentSocketConfig agentSocketConfig, Library.Logger.ILogger logger, AgentManager agentManager, ServerAgentActionManager serverAgentActionManager, RequestReceivedHandler requestReceivedHandler)
        {
            switch(agentSocketConfig.Protocol)
            {
                case "tcp": return new TCPAgentSocketsHandler(logger, agentSocketConfig.Port, agentManager, serverAgentActionManager, requestReceivedHandler);
            }
            return null;
        }
    }
}
