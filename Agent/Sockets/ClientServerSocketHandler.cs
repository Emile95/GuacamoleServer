using Agent.ServerApplication;
using Agent.ServerApplication.Request;
using API.Agent;
using Common;
using Common.Request;
using Common.Sockets;
using SocketHandler.State;
using SocketHandler.Tcp;

namespace Agent.Sockets
{
    public class ClientServerSocketHandler : TCPClientSocketHandler
    {
        private readonly ServerRequestHandler _serverRequestHandler;
        private readonly ServerOperations _serverOperations;

        public ClientServerSocketHandler(ServerRequestHandler serverRequestHandler, ServerOperations serverOperations) 
        {
            _serverRequestHandler = serverRequestHandler;
            _serverOperations = serverOperations;
        }

        protected override void OnLostSocket(ServerReceivedState receivedState)
        {
            Console.WriteLine("Lost unexpectectly server socket");
        }

        protected override void OnConnect(ServerReceivedState state)
        {
            AgentDefinition agentDefinition = new AgentDefinition
            {
                Name = Configuration.AgentName,
                Labels = Configuration.Labels,
                ConcurrentRun = (int)Configuration.ConcurrentRun
            };

            _serverOperations.BindSendAction((data) => Send(state.WorkSocket, data));

            _serverOperations.ConnectAgent(agentDefinition);
        }

        protected override void OnReceive(ServerReceivedState receivedState)
        {
            SocketRequestContext context = new SocketRequestContext();
            context.SourceSocket = receivedState.WorkSocket;
            context.Data = receivedState.BytesRead;
            _serverRequestHandler.ProcessRequest(context);
        }
    }
}
