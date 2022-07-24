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
            Socket socket = state.workSocket;

            int bytesRead = 0;
            try
            {
                bytesRead = socket.EndReceive(ar);
                StateObject newStateObject = new StateObject();
                newStateObject.workSocket = socket;
                socket.BeginReceive(newStateObject.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallBack), newStateObject);
            }
            catch (Exception e)
            {
                return;
            }

            SocketRequestContext context = new SocketRequestContext();
            context.SourceSocket = socket;
            context.NbByteReceived = bytesRead;

            context.Data = new byte[bytesRead];
            for (int i = 0; i < bytesRead; i++)
                context.Data[i] = state.buffer[i];

            state = new StateObject();
            state.workSocket = socket;

            _serverRequestReceivedHandler.ProcessRequest(context);
        }
    }
}
