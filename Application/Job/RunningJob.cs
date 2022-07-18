using Library.Agent;
using Library.Agent.Request.DataModel;


namespace Application.Job
{
    public class RunningJob
    {
        public string Id { get; set; }
        public AgentClient RunningOnAgent { get; set; }
        public JobStartDataModel JobRun { get; set; }
    }
}
