﻿using Agent.ServerApplication;
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
        private readonly ServerOperations _serverOperations;

        public ClientServerSocketHandler(System.Net.Sockets.Socket socket, int receivedBufferSize, ServerRequestHandler serverRequestHandler, ServerOperations serverOperations) 
            : base(socket, receivedBufferSize)
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

            _serverOperations.BindSendAction(state.SendHandler.Send);
            byte[] data = SocketRequestDataBytesBuilder.BuildRequestDataBytes(ApplicationConstValue.CONNECTAGENTREQUESTID, agentDefinition);
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