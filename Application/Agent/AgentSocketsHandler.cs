using Application.Agent.Request;
using System.Net;
using System.Net.Sockets;

namespace Application.Agent
{
    public abstract class AgentSocketsHandler
    {
        private readonly Application.Logger.ILogger _logger;
        private readonly AgentManager _agentManager;
        private readonly RequestHandler _requestHandler;

        protected readonly IPAddress _hostIpAddress;
        protected readonly int _port;
        protected readonly Socket _socket;

        public AgentSocketsHandler(Application.Logger.ILogger logger, int port)
        {
            _port = port;
            _hostIpAddress = Dns.GetHostAddresses(Dns.GetHostName())[0];
            _socket = GetSocket();
            _socket.Bind(GetEndpoint());
            _socket.Listen(100);
            _logger = logger;
            _agentManager = new AgentManager(logger);
            _requestHandler = new RequestHandler(logger);
        }

        public void Start()
        {
            _socket.BeginAccept(new AsyncCallback(AcceptCallBack), _socket);
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
            }
            catch (Exception e)
            {
                _agentManager.LostUnexpeclyAgentSocket(clientSocket);
                return;
            }

            RequestContext context = new RequestContext();
            context.AgentManager = _agentManager;
            context.SourceSocket = clientSocket;
            context.Data = state.buffer;

            _requestHandler.ProcessRequest(context);

            clientSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallBack), state);
        }

        protected abstract Socket GetSocket();

        protected abstract EndPoint GetEndpoint();
    }
}
