using System.Net.Sockets;

namespace Agent.ServerApplication.Request
{
    public class ServerRequestReceivedContext
    {
        public Socket ServerSocket { get; set; }
        public byte[] Data { get; set; }
    }
}
