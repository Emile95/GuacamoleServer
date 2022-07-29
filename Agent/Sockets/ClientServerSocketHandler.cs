using Agent.ServerApplication.Request;
using API.Agent;
using Common;
using Common.Request;
using Common.Sockets;
using SocketHandler;

namespace Agent.Sockets
{
    public class ClientServerSocketHandler : ClientSocketHandler
    {
        private readonly ServerRequestHandler _serverRequestHandler;
        private readonly AgentDefinition _agentDefinition;

        public ClientServerSocketHandler(System.Net.Sockets.Socket socket, int receivedBufferSize, ServerRequestHandler serverRequestHandler, AgentDefinition agentDefinition) 
            : base(socket, receivedBufferSize)
        {
            _serverRequestHandler = serverRequestHandler;
            _agentDefinition = agentDefinition;
        }

        protected override void OnLostSocket(ServerReceivedState receivedState)
        {
            Console.WriteLine("Lost unexpectectly server socket");
        }

        protected override void OnConnect(ServerReceivedState state)
        {
            byte[] data = SocketRequestDataBytesBuilder.BuildRequestDataBytes(ApplicationConstValue.CONNECTAGENTREQUESTID, _agentDefinition);
            state.SendHandler.Send(data);
        }

        protected override void OnReceive(ServerReceivedState receivedState)
        {
            SocketRequestContext context = new SocketRequestContext();
            context.SourceSocket = receivedState.WorkSocket;
            context.NbByteReceived = receivedState.NbBytesRead;
            context.Data = receivedState.BytesRead;
            _serverRequestHandler.ProcessRequest(context);
        }
    }
}
