using Application.Agent;
using Application.Agent.Request.DataModel;

namespace Application.Job
{
    public class RunningJob
    {
        public string Id { get; set; }
        public AgentClient RunningOnAgent { get; set; }
        public JobRunDataModel JobRun { get; set; }
    }
}
