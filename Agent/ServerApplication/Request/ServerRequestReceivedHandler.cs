using Agent.Action;
using Library;
using Library.Agent.Application;
using Library.Agent.Configuration.Application.AgentAction;
using Library.Agent.Request;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace Agent.ServerApplication.Request
{
    public class ServerRequestReceivedHandler
    {
        private readonly AgentActionManager _clientAgentActionManager;
        private readonly AgentApplicationManager _agentApplicationManager;

        public ServerRequestReceivedHandler(AgentActionManager clientAgentActionManager, AgentApplicationManager agentApplicationManager)
        {
            _clientAgentActionManager = clientAgentActionManager;
            _agentApplicationManager = agentApplicationManager;
        }

        public void ProcessRequest(ServerRequestReceivedContext context)
        {
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.TypeNameHandling = TypeNameHandling.All;

            AgentRequest agentRequest = JsonConvert.DeserializeObject<AgentRequest>(Encoding.ASCII.GetString(context.Data), setting);

            if (agentRequest.RequestId == ApplicationConstValue.INSTALLMODULERAGENTREQUESTID)
            {
                InstallModule(agentRequest.Data as ServerAgentApplicationLoaded, context);
                return;
            }

            if(_clientAgentActionManager.IsValidActionId(agentRequest.RequestId))
                _clientAgentActionManager.ProcessAction(agentRequest.RequestId, agentRequest.Data as string);
        }

        private void InstallModule(ServerAgentApplicationLoaded serverAgentApplicationLoaded, ServerRequestReceivedContext context)
        {
            string newDllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "agentApps", Path.GetFileName(serverAgentApplicationLoaded.FilePath));
            File.WriteAllBytes(newDllPath, serverAgentApplicationLoaded.FileBinary);
            _agentApplicationManager.InstallApplication(newDllPath, serverAgentApplicationLoaded.ActionIds);
        }
    }
}
