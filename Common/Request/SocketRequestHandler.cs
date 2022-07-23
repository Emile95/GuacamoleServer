using Common.Sockets;
using Newtonsoft.Json;
using System.Text;

namespace Common.Request
{
    public abstract class SocketRequestHandler
    {
        public void ProcessRequest(SocketRequestContext context)
        {
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.TypeNameHandling = TypeNameHandling.All;

            SocketRequest agentRequest = JsonConvert.DeserializeObject<SocketRequest>(Encoding.ASCII.GetString(context.Data), setting);

            ResolveSocketRequest(context, agentRequest);
        }

        protected abstract void ResolveSocketRequest(SocketRequestContext context, SocketRequest agentRequest);
    }
}
