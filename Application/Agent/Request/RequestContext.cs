﻿using System.Net.Sockets;

namespace Application.Agent.Request
{
    public class RequestContext
    {
        public AgentManager AgentManager { get; set; }
        public Socket SourceSocket { get; set; }
        public byte[] Data { get; set; }
    }
}
