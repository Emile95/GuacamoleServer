using Library;
using Library.Agent.Action;
using Library.Agent.Request;
using Newtonsoft.Json;
using System.Text;

namespace GuacamoleAgent.ServerApplication.Request
{
    public class RequestReceivedHandler
    {
        public void ProcessRequest(RequestReceivedContext context)
        {
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.TypeNameHandling = TypeNameHandling.All;

            Console.WriteLine(Encoding.ASCII.GetString(context.Data));

            AgentRequest agentRequest = JsonConvert.DeserializeObject<AgentRequest>(Encoding.ASCII.GetString(context.Data), setting);

            Console.WriteLine("a");
            if (agentRequest.RequestId == ApplicationConstValue.INSTALLMODULERAGENTREQUESTID)
            {
                Console.WriteLine("b");
                InstallModule(agentRequest.Data as AgentActionLoaded<Tuple<string, byte[]>>, context);
                return;
            }

            if(context.AgentActionManager.IsActionIdValid(agentRequest.RequestId))
                context.AgentActionManager.ProcessAction(agentRequest.RequestId);
        }

        private void InstallModule(AgentActionLoaded<Tuple<string, byte[]>> actionLoaded, RequestReceivedContext context)
        {
            Console.WriteLine("install module");
            /*AgentActionLoaded<AgentAction> agentActionLoaded = null;
            context.AgentActionManager.AddAgentAction(agentActionLoaded);*/
        }
    }
}
