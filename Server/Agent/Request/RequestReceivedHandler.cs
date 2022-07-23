using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Library.Agent;
using Library.Agent.Request;
using Library;
using Server.Agent.Action;

namespace Server.Agent.Request
{
    public class RequestReceivedHandler
    {
        private readonly Library.Logger.ILogger _logger;
        private readonly AgentApplicationManager _agentApplicationManager;
        private readonly ServerAgentActionManager _serverAgentActionManager;
        private readonly AgentManager _agentManager;

        public RequestReceivedHandler(Library.Logger.ILogger logger, AgentApplicationManager agentApplicationManager, ServerAgentActionManager serverAgentActionManager, AgentManager agentManager)
        {
            _logger = logger;
            _agentApplicationManager = agentApplicationManager;
            _serverAgentActionManager = serverAgentActionManager;
            _agentManager = agentManager;
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
            AgentClient agentClient = _agentManager.AddAgent(agentDefinition, context.SourceSocket);
            _logger.Log("Agent " + agentDefinition.Name + " connected");

            foreach(ServerAgentApplicationLoaded serverAgentApplicationLoaded in _agentApplicationManager.GetAgentApplicationLoadeds())
                agentClient.InstallAgentApplication(serverAgentApplicationLoaded);
        }

        private void AgentActionFinish(RequestReceivedContext context, string runningActionId)
        {
            _serverAgentActionManager.RemoveRunningAction(runningActionId);
            _logger.Log("Request finish, id : " + runningActionId);
        }
    }
}
