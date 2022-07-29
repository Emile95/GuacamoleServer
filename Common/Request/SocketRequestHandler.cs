using Common.Sockets;
using Newtonsoft.Json;
using System.Text;

namespace Common.Request
{
    public abstract class SocketRequestHandler<SocketRequestContextType>
        where SocketRequestContextType : SocketRequestContext
    {
        public void ProcessRequest(SocketRequestContextType context)
        {
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.TypeNameHandling = TypeNameHandling.All;

            SocketRequest agentRequest = JsonConvert.DeserializeObject<SocketRequest>(Encoding.ASCII.GetString(context.Data), setting);

            ResolveSocketRequest(context, agentRequest);
        }

        protected abstract void ResolveSocketRequest(SocketRequestContextType context, SocketRequest agentRequest);
    }
}
