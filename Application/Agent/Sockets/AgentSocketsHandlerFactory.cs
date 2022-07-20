using Application.Agent.Action;
using Application.Agent.Request;
using Application.Config;

namespace Application.Agent.Sockets
{
    public static class AgentSocketsHandlerFactory
    {
        public static AgentSocketsHandler CreateAgentSocketsHandler(SocketConfig socketConfig , Library.Logger.ILogger logger, AgentManager agentManager, ServerAgentActionManager serverAgentActionManager, RequestReceivedHandler requestReceivedHandler)
        {
            switch(socketConfig.Protocol)
            {
                case "tcp": return new TCPAgentSocketsHandler(logger, socketConfig.Port, agentManager, serverAgentActionManager, requestReceivedHandler);
            }
            return null;
        }
    }
}
