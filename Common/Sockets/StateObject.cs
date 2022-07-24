using System.Net.Sockets;

namespace Common.Sockets
{
    public class StateObject
    {
        // Size of receive buffer.  
        public const int BufferSize = 1000000;

        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];

        // Client socket.
        public Socket workSocket = null;

        public void ClearBuffer()
        {
            buffer = new byte[BufferSize];
        }
    }
}
