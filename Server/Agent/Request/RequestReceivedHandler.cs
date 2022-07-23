using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Library.Agent;
using Library.Agent.Request;
using Library;

namespace Server.Agent.Request
{
    public class RequestReceivedHandler
    {
        private readonly Library.Logger.ILogger _logger;
        private readonly AgentApplicationManager _agentApplicationManager;

        public RequestReceivedHandler(Library.Logger.ILogger logger, AgentApplicationManager agentApplicationManager)
        {
            _logger = logger;
            _agentApplicationManager = agentApplicationManager;
        }

        public void ProcessRequest(RequestReceivedContext context)
        {
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.TypeNameHandling = TypeNameHandling.All;

            AgentRequest agentRequest = JsonConvert.DeserializeObject<AgentRequest>(Encoding.ASCII.GetString(context.Data), setting);

            if (agentRequest.RequestId == ApplicationConstValue.CONNECTAGENTREQUESTID)
            {
                JObject jObject = (JObject)agentRequest.Data;
                ConnectAgent(jObject.ToObject<AgentDefinition>(), context);
                return;
            }

            if (agentRequest.RequestId == ApplicationConstValue.AGENTACTIONFINISHQUESTID)
            {
                AgentActionFinish(context, agentRequest.Data as string);
                return;
            }
        }

        private void ConnectAgent(AgentDefinition agentDefinition, RequestReceivedContext context)
        {
            AgentClient agentClient = context.AgentManager.AddAgent(agentDefinition, context.SourceSocket);
            _logger.Log("Agent " + agentDefinition.Name + " connected");

            foreach(ServerAgentApplicationLoaded serverAgentApplicationLoaded in _agentApplicationManager.GetAgentApplicationLoadeds())
                agentClient.InstallAgentApplication(serverAgentApplicationLoaded);
        }

        private void AgentActionFinish(RequestReceivedContext context, string runningActionId)
        {
            _logger.Log("Request finish, id : " + runningActionId);
        }
    }
}
