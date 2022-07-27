using Agent.Application;
using Common;
using Common.Request;
using Common.Sockets;
using Agent.AgentAction;
using API.Logging;

namespace Agent.ServerApplication.Request
{
    public class ServerRequestHandler : SocketRequestHandler<ServerSocketHandler>
    {
        private readonly SocketRequestLoggers _socketRequestLoggers;
        private readonly AgentActionManager _clientAgentActionManager;
        private readonly AgentApplicationManager _agentApplicationManager;

        public ServerRequestHandler(SocketRequestLoggers socketRequestLoggers, AgentActionManager clientAgentActionManager, AgentApplicationManager agentApplicationManager)
        {
            _socketRequestLoggers = socketRequestLoggers;
            _clientAgentActionManager = clientAgentActionManager;
            _agentApplicationManager = agentApplicationManager;
        }

        protected override void ResolveSocketRequest(SocketRequestContext<ServerSocketHandler> context, SocketRequest agentRequest)
        {
            _socketRequestLoggers.Log("socket request of type id : " + agentRequest.RequestId);

            if (agentRequest.RequestId == ApplicationConstValue.INSTALLMODULERAGENTREQUESTID)
            {
                InstallModule(context, agentRequest.Data as List<AgentApplicationLoaded>);
                return;
            }

            if (_clientAgentActionManager.IsValidActionId(agentRequest.RequestId))
                _clientAgentActionManager.ProcessAction(agentRequest.RequestId, agentRequest.Data as Tuple<string,object>);
        }

        private void InstallModule(SocketRequestContext<ServerSocketHandler> context, List<AgentApplicationLoaded> serverAgentApplicationLoadeds)
        {
            foreach (AgentApplicationLoaded serverAgentApplicationLoaded in serverAgentApplicationLoadeds)
            {
                string newDllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "agentApps", Path.GetFileName(serverAgentApplicationLoaded.FilePath));
                File.WriteAllBytes(newDllPath, serverAgentApplicationLoaded.FileBinary);
                _agentApplicationManager.InstallApplication(newDllPath, serverAgentApplicationLoaded.ActionIds);
            }
        }
    }
}
