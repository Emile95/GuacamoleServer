using System.Net;
using System.Net.Sockets;

namespace GuacamoleAgent.ServerApplication
{
    public static class ServerSocketFactory
    {
        public static Socket CreateServerSocket(string protocol, int port)
        {
            ProtocolType protocolType = 0;
            switch(protocol)
            {
                case "tcp": protocolType = ProtocolType.Tcp; break;
                default: throw new Exception("need to provide a valid protocol for socket, you provde '" + protocol + "'");
            }

            IPAddress host = Dns.GetHostAddresses(Dns.GetHostName())[0];

            Socket serverSocket = new Socket(
                host.AddressFamily,
                SocketType.Stream,
                protocolType
            );

            serverSocket.Connect(new IPEndPoint(host, port));

            return serverSocket;
        }
    }
}
