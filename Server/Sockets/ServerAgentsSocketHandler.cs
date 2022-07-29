using Common;
using Common.Sockets;
using Server.Agent;
using SocketHandler;
using System.Net.Sockets;

namespace Server.Sockets
{
    public class ServerAgentsSocketHandler : ServerSocketHandler<string>
    {
        private readonly AgentRequestHandler _agentRequestHandler;
        private readonly AgentManager _agentManager;

        public ServerAgentsSocketHandler(Socket socket, int receivedBufferSize, AgentRequestHandler agentRequestHandler, AgentManager agentManager) 
            : base(socket, receivedBufferSize) 
        {
            _agentRequestHandler = agentRequestHandler;
            _agentManager = agentManager;
        }

        protected override string GenerateClientKey(List<string> currentKeys)
        {
            return UniqueIdGenerator.Generate(currentKeys);
        }

        protected override void OnAccept(ClientReceivedState<string> receivedState) {  }

        protected override void OnLostSocket(ClientReceivedState<string> receivedState)
        {
            _agentManager.LostUnexpeclyAgentSocket(receivedState.ClientKey);
        }

        protected override void OnReceive(ClientReceivedState<string> receivedState)
        {
            AgentSocketRequestContext context = new AgentSocketRequestContext();
            context.SourceSocket = receivedState.WorkSocket;
            context.NbByteReceived = receivedState.NbBytesRead;
            context.Data = receivedState.BytesRead;
            context.AgentId = receivedState.ClientKey;
            context.sendToSocketAction = (data) => receivedState.SendHandler.Send(data);
            _agentRequestHandler.ProcessRequest(context);
        }
    }
}
