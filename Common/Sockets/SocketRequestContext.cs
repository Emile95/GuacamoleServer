using System.Net.Sockets;

namespace Common.Sockets
{
    public class SocketRequestContext<SocketHandlerType>
    {
        public SocketHandlerType SocketHandler { get; set; }
        public Socket SourceSocket { get; set; }
        public byte[] Data { get; set; }
        public int NbByteReceived { get; set; } = 0;
    }
}
