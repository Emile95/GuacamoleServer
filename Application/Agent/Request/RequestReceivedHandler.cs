using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Library.Agent;
using Library.Agent.Request;
using Library;
using Library.Agent.Action;

namespace Application.Agent.Request
{
    public class RequestReceivedHandler
    {
        private readonly Library.Logger.ILogger _logger;

        public RequestReceivedHandler(Library.Logger.ILogger logger)
        {
            _logger = logger;
        }

        public void ProcessRequest(RequestReceivedContext context)
        {
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.TypeNameHandling = TypeNameHandling.All;

            AgentRequest agentRequest = JsonConvert.DeserializeObject<AgentRequest>(Encoding.ASCII.GetString(context.Data), setting);

            JObject jObject = (JObject)agentRequest.Data;

            if(agentRequest.RequestId == ApplicationConstValue.CONNECTAGENTREQUESTID) ConnectAgent(jObject.ToObject<AgentDefinition>(), context); return;
        }

        private void ConnectAgent(AgentDefinition agentDefinition, RequestReceivedContext context)
        {
            AgentClient agentClient = context.AgentManager.AddAgent(agentDefinition, context.SourceSocket);
            _logger.Log("Agent " + agentDefinition.Name + " connected");
            List<AgentActionLoaded<Tuple<string, byte[]>>> data = context.ServerAgentActionManager.GetLoadedAgentActions();
            foreach(AgentActionLoaded<Tuple<string, byte[]>> agentActionLoaded in data)
                agentClient.InstallAgentAction(agentActionLoaded);
        }
    }
}
