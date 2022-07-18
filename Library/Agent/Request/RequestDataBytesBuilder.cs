using Newtonsoft.Json;
using System.Text;

namespace Library.Agent.Request
{
    public static class RequestDataBytesBuilder
    {

        public static byte[] BuildRequestDataBytes(string requestId, object data)
        {
            AgentRequest agentRequestData = new AgentRequest
            {
                RequestId = requestId,
                Data = data
            };

            string json = JsonConvert.SerializeObject(agentRequestData);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
