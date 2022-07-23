using AgentAction;
using Agent.Application;
using Common;
using Common.Request;
using Common.Sockets;
using Newtonsoft.Json;
using System.Text;

namespace Agent.ServerApplication.Request
{
    public class ServerRequestHandler
    {
        private readonly AgentActionManager _clientAgentActionManager;
        private readonly AgentApplicationManager _agentApplicationManager;

        public ServerRequestHandler(AgentActionManager clientAgentActionManager, AgentApplicationManager agentApplicationManager)
        {
            _clientAgentActionManager = clientAgentActionManager;
            _agentApplicationManager = agentApplicationManager;
        }

        public void ProcessRequest(SocketRequestContext context)
        {
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.TypeNameHandling = TypeNameHandling.All;

            SocketRequest agentRequest = JsonConvert.DeserializeObject<SocketRequest>(Encoding.ASCII.GetString(context.Data), setting);

            if (agentRequest.RequestId == ApplicationConstValue.INSTALLMODULERAGENTREQUESTID)
            {
                InstallModule(context, agentRequest.Data as AgentApplicationLoaded);
                return;
            }

            if(_clientAgentActionManager.IsValidActionId(agentRequest.RequestId))
                _clientAgentActionManager.ProcessAction(agentRequest.RequestId, agentRequest.Data as string);
        }

        private void InstallModule(SocketRequestContext context, AgentApplicationLoaded serverAgentApplicationLoaded)
        {
            string newDllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "agentApps", Path.GetFileName(serverAgentApplicationLoaded.FilePath));
            File.WriteAllBytes(newDllPath, serverAgentApplicationLoaded.FileBinary);
            _agentApplicationManager.InstallApplication(newDllPath, serverAgentApplicationLoaded.ActionIds);
        }
    }
}
