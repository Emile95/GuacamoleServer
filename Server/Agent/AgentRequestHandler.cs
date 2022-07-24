using Newtonsoft.Json.Linq;
using Server.AgentAction;
using Common.Sockets;
using Server.Application;
using Common;
using Common.Request;
using API.Agent;
using API.AgentAction;
using API.Logging;

namespace Server.Agent
{
    public class AgentRequestHandler : SocketRequestHandler
    {
        private readonly SocketRequestLoggers _socketRequestLoggers;
        private readonly AgentApplicationManager _agentApplicationManager;
        private readonly AgentActionManager _serverAgentActionManager;
        private readonly AgentManager _agentManager;

        public AgentRequestHandler(SocketRequestLoggers socketRequestLoggers, AgentApplicationManager agentApplicationManager, AgentActionManager serverAgentActionManager, AgentManager agentManager)
        {
            _socketRequestLoggers = socketRequestLoggers;
            _agentApplicationManager = agentApplicationManager;
            _serverAgentActionManager = serverAgentActionManager;
            _agentManager = agentManager;
        }

        protected override void ResolveSocketRequest(SocketRequestContext context, SocketRequest agentRequest)
        {
            _socketRequestLoggers.Log("socket request of type id : " + agentRequest.RequestId);

            Console.WriteLine(context.NbByteReceived.ToString());

            if (agentRequest.RequestId == ApplicationConstValue.CONNECTAGENTREQUESTID)
            {
                JObject jObject = (JObject)agentRequest.Data;
                ConnectAgent(context, jObject.ToObject<AgentDefinition>());
                return;
            }

            if (agentRequest.RequestId == ApplicationConstValue.AGENTACTIONLOGREQUESTID)
            {
                RunningAgentActionLog(context, agentRequest.Data as RunningAgentActionLog);
                return;
            }
        }

        private void ConnectAgent(SocketRequestContext context, AgentDefinition agentDefinition)
        {
            AgentClient agentClient = _agentManager.AddAgent(agentDefinition, context.SourceSocket);
            agentClient.InstallAgentApplications(_agentApplicationManager.GetAgentApplicationLoadeds());
        }

        private void RunningAgentActionLog(SocketRequestContext context, RunningAgentActionLog runningAgentActionLog)
        {
            _serverAgentActionManager.LogRunningAgentAction(runningAgentActionLog);
        }
    }
}
