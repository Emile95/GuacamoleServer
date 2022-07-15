using System.Net;
using System.Net.Sockets;


namespace Application.Sockets
{
    public abstract class ServerSocket
    {
        protected readonly int _port;
        protected readonly Socket _socket;

        private readonly List<Socket> _clientSockets;
        private readonly Application.Logger.ILogger _logger;

        public ServerSocket(Application.Logger.ILogger logger, int port)
        {
            _port = port;
            _socket = GetSocket();
            _socket.Bind(GetEndpoint());
            _socket.Listen(100);
            _clientSockets = new List<Socket>();
            _logger = logger;

            
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
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            _clientSockets.Add(handler);

            _logger.Log("New Client connected");

            StateObject state = new StateObject();
            state.workSocket = handler;

            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallBack), state);

            listener.BeginAccept(new AsyncCallback(AcceptCallBack), _socket);
        }

        private void ReadCallBack(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            try
            {
                int bytesRead = handler.EndReceive(ar);
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallBack), state);
            } catch(Exception e)
            {
                _logger.Log("Client Lost unexpectly");
                _clientSockets.Remove(handler);
            }
        }

        protected abstract Socket GetSocket();

        protected abstract EndPoint GetEndpoint();
    }
}
