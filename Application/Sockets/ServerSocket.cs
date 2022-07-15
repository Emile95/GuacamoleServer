using System.Net;
using System.Net.Sockets;

namespace Application.Sockets
{
    public abstract class ServerSocket
    {
        protected readonly int _port;
        protected readonly Socket _socket;

        public ServerSocket(int port)
        {
            _port = port;
            _socket = GetSocket();
            _socket.Bind(GetEndpoint());
            _socket.Listen(100);
        }

        public void Start()
        {
            _socket.BeginAccept((asyncResult) => {

            }, _socket);
        }

        public void Stop()
        {
            _socket.Close();
        }

        protected abstract Socket GetSocket();

        protected abstract EndPoint GetEndpoint();
    }
}
