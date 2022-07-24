using API.Logging;
using Server.Config;

namespace Server.Agent.Sockets
{
    public static class AgentSocketsHandlerFactory
    {
        public static AgentSocketsHandler CreateAgentSocketsHandler(AgentsSocketConfig agentSocketsConfig, SocketLoggers socketLoggers, AgentManager agentManager, AgentRequestHandler requestReceivedHandler)
        {
            switch(agentSocketsConfig.Protocol)
            {
                case "tcp": return new TCPAgentSocketsHandler(socketLoggers, agentSocketsConfig.Port, agentManager, requestReceivedHandler);
            }
            throw new Exception("most prodive a valid string expression for socket protocol : '" + agentSocketsConfig.Protocol + "' is Invalid");
        }
    }
}
