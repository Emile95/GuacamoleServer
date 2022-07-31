using Common;
using Server.Agent;
using SocketHandler.State;
using SocketHandler.Tcp;

namespace Server.Sockets
{
    public class ServerAgentsSocketHandler : TCPServerSocketHandler<string>
    {
        private readonly AgentRequestHandler _agentRequestHandler;
        private readonly AgentManager _agentManager;

        public ServerAgentsSocketHandler(AgentRequestHandler agentRequestHandler, AgentManager agentManager) 
        {
            _agentRequestHandler = agentRequestHandler;
            _agentManager = agentManager;
        }

        protected override string GenerateClientKey(List<string> currentKeys)
        {
            return UniqueIdGenerator.Generate(currentKeys);
        }

        protected override void OnAccept(ClientReceivedState<string> receivedState)
        {
            
        }

        protected override void OnLostSocket(ClientReceivedState<string> receivedState)
        {
            _agentManager.LostUnexpeclyAgentSocket(receivedState.ClientKey);
        }

        protected override void OnReceive(ClientReceivedState<string> receivedState)
        {
            AgentSocketRequestContext context = new AgentSocketRequestContext();
            context.SourceSocket = receivedState.WorkSocket;
            context.Data = receivedState.BytesRead;
            context.AgentId = receivedState.ClientKey;
            context.sendToSocketAction = (data) => Send(receivedState.WorkSocket, data);
            _agentRequestHandler.ProcessRequest(context);
        }
    }
}
