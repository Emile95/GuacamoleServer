using API.Logging;
using Common.Sockets;
using System.Net;
using System.Net.Sockets;

namespace Server.Agent.Sockets
{
    public abstract class AgentSocketsHandler
    {
        private readonly SocketLoggers _socketLoggers;
        private readonly AgentManager _agentManager;
        private readonly AgentRequestHandler _agentRequestReceivedHandler;

        protected readonly IPAddress _hostIpAddress;
        protected readonly int _port;
        protected readonly Socket _socket;

        public AgentSocketsHandler(SocketLoggers socketLoggers, int port, AgentManager agentManager, AgentRequestHandler agentRequestReceivedHandler)
        {
            _port = port;
            _hostIpAddress = Dns.GetHostAddresses(Dns.GetHostName())[0];
            _socket = GetSocket();
            _socket.Bind(GetEndpoint());
            _socket.Listen(100);
            _socketLoggers = socketLoggers;
            _agentManager= agentManager;
            _agentRequestReceivedHandler = agentRequestReceivedHandler;
        }

        public void Start()
        {
            _socket.BeginAccept(new AsyncCallback(AcceptCallBack), _socket);
            _socketLoggers.Log("Socket listening at " + _hostIpAddress.ToString() + ":" + _port);
        }

        public void Stop()
        {
            _socket.Close();
        }

        private void AcceptCallBack(IAsyncResult ar)
        {
            Socket serverSocket = (Socket)ar.AsyncState;
            Socket clientSocket = serverSocket.EndAccept(ar);

            StateObject state = new StateObject();
            state.workSocket = clientSocket;

            clientSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallBack), state);

            serverSocket.BeginAccept(new AsyncCallback(AcceptCallBack), _socket);
        }

        private void ReadCallBack(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket clientSocket = state.workSocket;

            int bytesRead = 0;
            try
            {
                bytesRead  = clientSocket.EndReceive(ar);
                StateObject newStateObject = new StateObject();
                newStateObject.workSocket = clientSocket;
                clientSocket.BeginReceive(newStateObject.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallBack), newStateObject);
            }
            catch (Exception e)
            {
                _agentManager.LostUnexpeclyAgentSocket(clientSocket);
                return;
            }

            SocketRequestContext context = new SocketRequestContext();
            context.SourceSocket = clientSocket;
            context.NbByteReceived = bytesRead;

            context.Data = new byte[bytesRead];
            for (int i = 0; i < bytesRead; i++)
                context.Data[i] = state.buffer[i];

            state = new StateObject();
            state.workSocket = clientSocket;

            _agentRequestReceivedHandler.ProcessRequest(context);
        }

        protected abstract Socket GetSocket();

        protected abstract EndPoint GetEndpoint();
    }
}
