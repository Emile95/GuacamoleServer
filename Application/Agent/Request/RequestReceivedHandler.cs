using Library.Agent.Request.DataModel;
using Application.Job;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Library.Agent;
using Library.Agent.Request;

namespace Application.Agent.Request
{
    public class RequestReceivedHandler
    {
        private readonly Library.Logger.ILogger _logger;

        public RequestReceivedHandler(Library.Logger.ILogger logger)
        {
            _logger = logger;
        }

        public void ProcessRequest(RequestReceivedContext context)
        {
            RequestData requestData = JsonConvert.DeserializeObject<RequestData>(Encoding.ASCII.GetString(context.Data));

            JObject jObject = (JObject)requestData.Data;

            switch (requestData.RequestType)
            {
                case RequestType.AgentConnect: ConnectAgent(jObject.ToObject<AgentDefinition>(), context); break;
                case RequestType.FinishJob: FinishJob(jObject.ToObject<JobStartDataModel>(), context); break;
            }
        }

        private void ConnectAgent(AgentDefinition agentDefinition, RequestReceivedContext context)
        {
            context.AgentManager.AddAgent(agentDefinition, context.SourceSocket);
            _logger.Log("Agent " + agentDefinition.Name + " connected");
        }

        private void FinishJob(JobStartDataModel jobRun, RequestReceivedContext context)
        {
            RunningJob runningJob = context.JobManager.GetRunningJob(jobRun.Id);
            runningJob.RunningOnAgent.DecrementJobRunning();
            context.JobManager.RemoveRunningJob(jobRun.Id);
            _logger.Log("job " + jobRun.Id + " is finished");
        }
    }
}
