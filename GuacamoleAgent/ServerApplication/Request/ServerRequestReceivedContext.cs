using System.Net.Sockets;

namespace GuacamoleAgent.ServerApplication.Request
{
    public class ServerRequestReceivedContext
    {
        public Socket ServerSocket { get; set; }
        public byte[] Data { get; set; }
    }
}
