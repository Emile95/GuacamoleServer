using System.Net;
using System.Net.Sockets;

namespace Agent.ServerApplication
{
    public static class ServerSocketFactory
    {
        public static Socket CreateServerSocket(string host, string protocol, int port)
        {
            ProtocolType protocolType = 0;
            switch(protocol)
            {
                case "tcp": protocolType = ProtocolType.Tcp; break;
                default: throw new Exception("need to provide a valid protocol for socket, you provde '" + protocol + "'");
            }

            IPAddress hostIpAddress = IPAddress.Parse(host);

#if DEBUG
            hostIpAddress = Dns.GetHostAddresses(Dns.GetHostName())[0];
#endif

            Socket serverSocket = new Socket(
                hostIpAddress.AddressFamily,
                SocketType.Stream,
                protocolType
            );

            serverSocket.Connect(new IPEndPoint(hostIpAddress, port));

            return serverSocket;
        }
    }
}
