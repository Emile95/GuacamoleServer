using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Application.Agent.Request.Received
{
    public class RequestReceivedHandler
    {
        private readonly Application.Logger.ILogger _logger;

        public RequestReceivedHandler(Application.Logger.ILogger logger)
        {
            _logger = logger;
        }

        public void ProcessRequest(RequestReceivedContext context)
        {
            AgentRequestData requestData = JsonConvert.DeserializeObject<AgentRequestData>(Encoding.ASCII.GetString(context.Data));

            JObject jObject = (JObject)requestData.Data;

            switch (requestData.RequestType)
            {
                case AgentRequestType.AgentConnect: ConnectAgent(jObject.ToObject<AgentDefinition>(), context); break;
            }
        }

        private void ConnectAgent(AgentDefinition agentDefinition, RequestReceivedContext context)
        {
            context.AgentManager.AddAgent(agentDefinition, context.SourceSocket);
            _logger.Log("Agent " + agentDefinition.Name + " connected");
        }
    }
}
