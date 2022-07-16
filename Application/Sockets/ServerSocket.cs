using System.Net;
using System.Net.Sockets;

namespace Application.Sockets
{
    public abstract class ServerSocket
    {
        protected readonly IPAddress _hostIpAddress;
        protected readonly int _port;
        protected readonly Socket _socket;

        private readonly List<Socket> _clientSockets;
        private readonly Application.Logger.ILogger _logger;

        private readonly SocketDataHandler _socketDataHandler;

        public ServerSocket(Application.Logger.ILogger logger, SocketDataHandler socketDataHandler, int port)
        {
            _port = port;
            _hostIpAddress = Dns.GetHostAddresses(Dns.GetHostName())[0];
            _socket = GetSocket();
            _socket.Bind(GetEndpoint());
            _socket.Listen(100);
            _clientSockets = new List<Socket>();
            _logger = logger;
            _socketDataHandler = socketDataHandler;
        }

        public void Start()
        {
            _socket.BeginAccept(new AsyncCallback(AcceptCallBack), _socket);
        }

        public void Stop()
        {
            _socket.Close();
            foreach (Socket socket in _clientSockets)
                socket.Close();
        }

        private void AcceptCallBack(IAsyncResult ar)
        {
            Socket serverSocket = (Socket)ar.AsyncState;
            Socket clientSocket = serverSocket.EndAccept(ar);

            _clientSockets.Add(clientSocket);

            _logger.Log("New Client connected");

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
                _logger.Log("Client Lost unexpectly");
                _clientSockets.Remove(clientSocket);
                return;
            }

            SocketData socketData = _socketDataHandler.CreateSocketDataFromBytes(state.buffer);

            clientSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallBack), state);
        }

        protected abstract Socket GetSocket();

        protected abstract EndPoint GetEndpoint();
    }
}
