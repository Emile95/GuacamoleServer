using Server.Agent.Action;
using System.Net.Sockets;

namespace Server.Agent.Request
{
    public class RequestReceivedContext
    {
        public AgentManager AgentManager { get; set; }
        public Socket SourceSocket { get; set; }
        public ServerAgentActionManager ServerAgentActionManager { get; set; }
        public byte[] Data { get; set; }
        public int NbByteReceived = 0;
    }
}
