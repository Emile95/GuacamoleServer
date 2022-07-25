using Agent.ServerApplication.Request;
using API.Agent;
using Common;
using Common.Request;
using Common.Sockets;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Text;

namespace Agent.ServerApplication
{
    public class ServerSocketHandler
    {
        private readonly Socket _serverSocket;
        private readonly ServerRequestHandler _serverRequestReceivedHandler;

        public ServerSocketHandler(Socket serverSocket, ServerRequestHandler serverRequestReceivedHandler)
        {
            _serverSocket = serverSocket;
            _serverRequestReceivedHandler = serverRequestReceivedHandler;
        }

        public void Start(AgentDefinition agentDefinition)
        {
            SocketRequest data = new SocketRequest
            {
                RequestId = ApplicationConstValue.CONNECTAGENTREQUESTID,
                Data = agentDefinition
            };

            _serverSocket.Send(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(data)));

            StateObject state = new StateObject();
            state.workSocket = _serverSocket;

            _serverSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallBack), state);
        }

        private void ReadCallBack(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket serverSocket = state.workSocket;

            int bytesRead = 0;
            try
            {
                bytesRead = serverSocket.EndReceive(ar);
                StateObject newStateObject = new StateObject();
                newStateObject.workSocket = serverSocket;
                serverSocket.BeginReceive(newStateObject.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallBack), newStateObject);

                SocketRequest socketRequest = new SocketRequest();
                socketRequest.RequestId = ApplicationConstValue.LIBERATESOCKETSENDREQUESTSID;
                socketRequest.Data = AgentDefinitionValues.AgentId;
                string requestJson = JsonConvert.SerializeObject(socketRequest);
                serverSocket.Send(Encoding.ASCII.GetBytes(requestJson));

            }
            catch (Exception e)
            {
                return;
            }

            SocketRequestContext<ServerSocketHandler> context = new SocketRequestContext<ServerSocketHandler>();
            context.SourceSocket = serverSocket;
            context.NbByteReceived = bytesRead;
            context.SocketHandler = this;

            context.Data = new byte[bytesRead];
            for (int i = 0;i < bytesRead; i++)
                context.Data[i] = state.buffer[i];

            _serverRequestReceivedHandler.ProcessRequest(context);
        }
    }
}
