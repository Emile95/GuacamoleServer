using GuacamoleAgent.Action;
using System.Net.Sockets;

namespace GuacamoleAgent.ServerApplication.Request
{
    public class RequestReceivedContext
    {
        public Socket ServerSocket { get; set; }
        public byte[] Data { get; set; }
        public AgentActionManager AgentActionManager { get; set; }
    }
}
