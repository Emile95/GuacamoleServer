using System.Net;
using System.Net.Sockets;

namespace Application.Sockets
{
    public class TCPIPSocket : ServerSocket
    {
        public TCPIPSocket(Application.Logger.ILogger logger, int port)
        : base(logger, port) {}

        protected override Socket GetSocket()
        {
            return new Socket(
                Dns.GetHostAddresses(Dns.GetHostName())[0].AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp
            );
        }

        protected override EndPoint GetEndpoint()
        {
            return new IPEndPoint(Dns.GetHostAddresses(Dns.GetHostName())[0], _port);
        }
    }
}
