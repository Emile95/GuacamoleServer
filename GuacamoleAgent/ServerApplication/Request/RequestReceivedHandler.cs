using Library;
using Library.Agent.Action;
using Library.Agent.Request;
using Newtonsoft.Json;
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
                InstallModule(agentRequest.Data as AgentActionLoaded, context);
                return;
            }

            if(context.AgentActionManager.IsActionIdValid(agentRequest.RequestId))
                context.AgentActionManager.ProcessAction(agentRequest.RequestId);
        }

        private void InstallModule(AgentActionLoaded agentActionLoaded, RequestReceivedContext context)
        {
            context.AgentActionManager.AddAgentAction(agentActionLoaded);
        }
    }
}
