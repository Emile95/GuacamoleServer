using System.Net;
using System.Net.Sockets;

namespace Common.Sockets
{
    public static class SocketFactory
    {
        public static Socket CreateSocket(string protocol, string host = null)
        {
            ProtocolType protocolType = 0;
            switch (protocol)
            {
                case "tcp": protocolType = ProtocolType.Tcp; break;
                default: throw new Exception("need to provide a valid protocol for socket, you provde '" + protocol + "'");
            }

            IPAddress hostIpAddress;
            if (host == null)
            {
                hostIpAddress = Dns.GetHostAddresses(Dns.GetHostName())[0];
            } else
            {
                hostIpAddress = IPAddress.Parse(host);
            }
#if DEBUG
            hostIpAddress = Dns.GetHostAddresses(Dns.GetHostName())[0];
#endif

            Socket socket = new Socket(
                hostIpAddress.AddressFamily,
                SocketType.Stream,
                protocolType
            );

            return socket;
        }
    }
}
