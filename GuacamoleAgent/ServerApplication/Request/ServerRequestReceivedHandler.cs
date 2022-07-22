using GuacamoleAgent.Action;
using Library;
using Library.Agent.Action;
using Library.Agent.Request;
using Newtonsoft.Json;
using System.Text;

namespace GuacamoleAgent.ServerApplication.Request
{
    public class ServerRequestReceivedHandler
    {
        private readonly ClientAgentActionManager _clientAgentActionManager;

        public ServerRequestReceivedHandler(ClientAgentActionManager clientAgentActionManager)
        {
            _clientAgentActionManager = clientAgentActionManager;
        }

        public void ProcessRequest(ServerRequestReceivedContext context)
        {
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.TypeNameHandling = TypeNameHandling.All;

            AgentRequest agentRequest = JsonConvert.DeserializeObject<AgentRequest>(Encoding.ASCII.GetString(context.Data), setting);

            if (agentRequest.RequestId == ApplicationConstValue.INSTALLMODULERAGENTREQUESTID)
            {
                InstallModule(agentRequest.Data as AgentActionLoaded<Tuple<string, byte[]>>, context);
                return;
            }

            if(_clientAgentActionManager.IsActionIdValid(agentRequest.RequestId))
                _clientAgentActionManager.ProcessAction(agentRequest.RequestId, agentRequest.Data as string);
        }

        private void InstallModule(AgentActionLoaded<Tuple<string, byte[]>> actionLoaded, ServerRequestReceivedContext context)
        {
            string newDllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "agentActions", actionLoaded.Instance.Item1);
            File.WriteAllBytes(newDllPath, actionLoaded.Instance.Item2);

            List<AgentAction> agentActions = PluginFactory.CreatePluginsFromFile<AgentAction>(newDllPath);

            foreach (AgentAction agentAction in agentActions)
            {
                AgentActionLoaded<AgentAction> action = new AgentActionLoaded<AgentAction>();
                action.ActionId = actionLoaded.ActionId;
                action.DisplayName = actionLoaded.DisplayName;
                action.Instance = agentAction;
                _clientAgentActionManager.AddAgentAction(action);
            }
        }
    }
}
