using System.Net.Sockets;

namespace Server.Agent.Request
{
    public class RequestReceivedContext
    {
        public Socket SourceSocket { get; set; }
        public byte[] Data { get; set; }
        public int NbByteReceived = 0;
    }
}
