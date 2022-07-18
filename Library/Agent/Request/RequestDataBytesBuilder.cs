using Newtonsoft.Json;
using System.Text;

namespace Library.Agent.Request
{
    public static class RequestDataBytesBuilder
    {

        public static byte[] BuildRequestDataBytes(RequestType requestType, object data)
        {
            RequestData agentRequestData = new RequestData
            {
                RequestType = requestType,
                Data = data
            };

            string json = JsonConvert.SerializeObject(agentRequestData);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
