using Agent.Application;
using Common;
using Common.Request;
using Common.Sockets;
using Agent.AgentAction;

namespace Agent.ServerApplication.Request
{
    public class ServerRequestHandler : SocketRequestHandler
    {
        private readonly AgentActionManager _clientAgentActionManager;
        private readonly AgentApplicationManager _agentApplicationManager;

        public ServerRequestHandler(AgentActionManager clientAgentActionManager, AgentApplicationManager agentApplicationManager)
        {
            _clientAgentActionManager = clientAgentActionManager;
            _agentApplicationManager = agentApplicationManager;
        }

        protected override void ResolveSocketRequest(SocketRequestContext context, SocketRequest agentRequest)
        {
            if (agentRequest.RequestId == ApplicationConstValue.INSTALLMODULERAGENTREQUESTID)
            {
                InstallModule(context, agentRequest.Data as AgentApplicationLoaded);
                return;
            }

            if (_clientAgentActionManager.IsValidActionId(agentRequest.RequestId))
                _clientAgentActionManager.ProcessAction(agentRequest.RequestId, agentRequest.Data as Tuple<string,object>);
        }

        private void InstallModule(SocketRequestContext context, AgentApplicationLoaded serverAgentApplicationLoaded)
        {
            string newDllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "agentApps", Path.GetFileName(serverAgentApplicationLoaded.FilePath));
            File.WriteAllBytes(newDllPath, serverAgentApplicationLoaded.FileBinary);
            _agentApplicationManager.InstallApplication(newDllPath, serverAgentApplicationLoaded.ActionIds);
        }
    }
}
