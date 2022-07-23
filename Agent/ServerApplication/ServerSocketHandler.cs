using Agent.ServerApplication.Request;
using Library;
using Library.Agent;
using Library.Agent.Request;
using Library.Sockets;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Text;

namespace Agent.ServerApplication
{
    public class ServerSocketHandler
    {
        private readonly int _port;
        private readonly Socket _serverSocket;
        private readonly ServerRequestReceivedHandler _serverRequestReceivedHandler;

        public ServerSocketHandler(Socket serverSocket, ServerRequestReceivedHandler serverRequestReceivedHandler)
        {
            _serverSocket = serverSocket;
            _serverRequestReceivedHandler = serverRequestReceivedHandler;
        }

        public void Start(AgentDefinition agentDefinition)
        {
            AgentRequest data = new AgentRequest
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
            }
            catch (Exception e)
            {
                return;
            }

            SocketRequestContext context = new SocketRequestContext();
            context.SourceSocket = socket;

            context.Data = new byte[bytesRead];
            for (int i = 0; i < bytesRead; i++)
                context.Data[i] = state.buffer[i];

            _serverRequestReceivedHandler.ProcessRequest(context);

            state.ClearBuffer();

            socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallBack), state);
        }
    }
}
