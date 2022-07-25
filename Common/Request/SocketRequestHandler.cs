using Common.Sockets;
using Newtonsoft.Json;
using System.Text;

namespace Common.Request
{
    public abstract class SocketRequestHandler<SocketHandlerType>
    {
        public void ProcessRequest(SocketRequestContext<SocketHandlerType> context)
        {
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.TypeNameHandling = TypeNameHandling.All;

            SocketRequest agentRequest = JsonConvert.DeserializeObject<SocketRequest>(Encoding.ASCII.GetString(context.Data), setting);

            ResolveSocketRequest(context, agentRequest);
        }

        protected abstract void ResolveSocketRequest(SocketRequestContext<SocketHandlerType> context, SocketRequest agentRequest);
    }
}
