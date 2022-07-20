using Library;
using Library.Agent.Action;
using Library.Agent.Request;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace GuacamoleAgent.ServerApplication.Request
{
    public class RequestReceivedHandler
    {
        public void ProcessRequest(RequestReceivedContext context)
        {
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.TypeNameHandling = TypeNameHandling.All;

            AgentRequest agentRequest = JsonConvert.DeserializeObject<AgentRequest>(Encoding.ASCII.GetString(context.Data), setting);

            if (agentRequest.RequestId == ApplicationConstValue.INSTALLMODULERAGENTREQUESTID)
            {
                InstallModule(agentRequest.Data as AgentActionLoaded<Tuple<string, byte[]>>, context);
                return;
            }

            if(context.AgentActionManager.IsActionIdValid(agentRequest.RequestId))
                context.AgentActionManager.ProcessAction(agentRequest.RequestId);
        }

        private void InstallModule(AgentActionLoaded<Tuple<string, byte[]>> actionLoaded, RequestReceivedContext context)
        {
            string newDllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "agentActions", actionLoaded.Instance.Item1);
            File.WriteAllBytes(newDllPath, actionLoaded.Instance.Item2);

            Assembly jihogoAssembly = Assembly.LoadFile(newDllPath);

            AgentAction jihoAgentAction = null;
            Type applicationType = typeof(AgentAction);

            foreach (Type type in jihogoAssembly.GetTypes())
            {
                if (applicationType.IsAssignableFrom(type))
                {
                    jihoAgentAction = Activator.CreateInstance(type) as AgentAction;
                    AgentActionDefinition definition = jihoAgentAction.GetAgentActionDefinition();
                    AgentActionLoaded<AgentAction> action = new AgentActionLoaded<AgentAction>();
                    action.ActionId = actionLoaded.ActionId;
                    action.DisplayName = actionLoaded.DisplayName;
                    action.Instance = jihoAgentAction;
                    context.AgentActionManager.AddAgentAction(action);
                }
            }
        }
    }
}
