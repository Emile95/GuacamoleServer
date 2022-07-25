using Newtonsoft.Json.Linq;
using Server.AgentAction;
using Common.Sockets;
using Server.Application;
using Common;
using Common.Request;
using API.Agent;
using API.AgentAction;
using API.Logging;
using Server.Agent.Sockets;

namespace Server.Agent
{
    public class AgentRequestHandler : SocketRequestHandler<AgentSocketsHandler>
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

        protected override void ResolveSocketRequest(SocketRequestContext<AgentSocketsHandler> context, SocketRequest agentRequest)
        {
            _socketRequestLoggers.Log("socket request of type id : " + agentRequest.RequestId);

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

            if (agentRequest.RequestId == ApplicationConstValue.LIBERATESOCKETSENDREQUESTSID)
            {
                LiberateAgentRequestSend(context, agentRequest.Data as string);
                return;
            }
        }

        private void LiberateAgentRequestSend(SocketRequestContext<AgentSocketsHandler> context, string agentId)
        {
            AgentClient agentClient  = _agentManager.GetAgentClientById(agentId);
            agentClient.LiberateRequestSend();
        }

        private void ConnectAgent(SocketRequestContext<AgentSocketsHandler> context, AgentDefinition agentDefinition)
        {
            AgentClient agentClient = _agentManager.AddAgent(agentDefinition, context.SourceSocket);
            foreach(AgentApplicationLoaded agentApplicationLoaded in _agentApplicationManager.GetAgentApplicationLoadeds())
                agentClient.InstallAgentApplication(agentApplicationLoaded);
        }

        private void RunningAgentActionLog(SocketRequestContext<AgentSocketsHandler> context, RunningAgentActionLog runningAgentActionLog)
        {
            _serverAgentActionManager.LogRunningAgentAction(runningAgentActionLog);
        }
    }
}
