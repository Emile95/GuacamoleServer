using Application.Job;
using System.Net.Sockets;

namespace Application.Agent.Request.Received
{
    public class RequestReceivedContext
    {
        public JobManager JobManager { get; set; }
        public AgentManager AgentManager { get; set; }
        public Socket SourceSocket { get; set; }
        public byte[] Data { get; set; }
    }
}
