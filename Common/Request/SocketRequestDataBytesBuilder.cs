using Newtonsoft.Json;
using System.Text;

namespace Common.Request
{
    public static class SocketRequestDataBytesBuilder
    {

        public static byte[] BuildRequestDataBytes(string requestId, object data)
        {
            
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.TypeNameHandling = TypeNameHandling.All;

            SocketRequest agentRequestData = new SocketRequest
            {
                RequestId = requestId,
                Data = data
            };

            string json = JsonConvert.SerializeObject(agentRequestData, setting);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
