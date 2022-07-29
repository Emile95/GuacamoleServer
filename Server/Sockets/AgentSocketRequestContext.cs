using Common.Sockets;

namespace Server.Sockets
{
    public class AgentSocketRequestContext : SocketRequestContext
    {
        public string AgentId { get; set; }
        public Action<byte[]> sendToSocketAction { get; set; }
    }
}
