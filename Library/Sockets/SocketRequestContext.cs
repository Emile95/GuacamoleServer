using System.Net.Sockets;

namespace Library.Sockets
{
    public class SocketRequestContext
    {
        public Socket SourceSocket { get; set; }
        public byte[] Data { get; set; }
        public int NbByteReceived = 0;
    }
}
