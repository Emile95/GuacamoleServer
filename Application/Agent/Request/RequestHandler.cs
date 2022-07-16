using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Application.Agent.Request
{
    public class RequestHandler
    {
        private readonly Application.Logger.ILogger _logger;

        public RequestHandler(Application.Logger.ILogger logger)
        {
            _logger = logger;
        }

        public void ProcessRequest(RequestContext context)
        {
            RequestData requestData = JsonConvert.DeserializeObject<RequestData>(Encoding.ASCII.GetString(context.Data));

            JObject jObject = (JObject)requestData.Data;

            switch (requestData.RequestType)
            {
                case RequestType.AgentConnect: ConnectAgent(jObject.ToObject<AgentDefinition>(), context); break;
            }
        }

        private void ConnectAgent(AgentDefinition agentDefinition, RequestContext context)
        {
            context.AgentManager.AddAgent(agentDefinition.Id, context.SourceSocket);
            _logger.Log("Agent " + agentDefinition.Name + " connected");
        }
    }
}
