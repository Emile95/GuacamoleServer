using Application.Agent;
using Application.Agent.Request;
using Application.Agent.Request.DataModel;
using Application.DataModel;
using Newtonsoft.Json;
using System.Text;

namespace Application.Job
{
    public class JobManager
    {
        private Application.Logger.ILogger _logger;

        private readonly AgentManager _agentManager;

        private Dictionary<string, RunningJob> _runningJobs;

        public JobManager(Application.Logger.ILogger logger, AgentManager agentManager)
        {
            _logger = logger;
            _agentManager = agentManager;
            _runningJobs = new Dictionary<string, RunningJob>();
        }

        private void AddRuningJob(RunningJob runningJob)
        {
            _runningJobs.Add(runningJob.Id, runningJob);
        }

        public RunningJob GetRunningJob(string id)
        {
            return _runningJobs[id];
        }

        public void RemoveRunningJob(string id)
        {
            _runningJobs.Remove(id);
        }

        private bool IsRunningJobIdAvailable(string id)
        {
            return _runningJobs.ContainsKey(id) == false;
        }

        public object StartJobOnAgent(StartJobDataModel job)
        {
            AgentClient foundedAgent = null;
            try
            {
                foundedAgent = _agentManager.GetAvailableAgentByLabel(job.AgentLabel);
            }
            catch (Exception e) { return e.Message; }

            JobRunDataModel jobRun = new JobRunDataModel();
            jobRun.AgentLabel = job.AgentLabel;
            jobRun.Script = "adsdasdasd";

            do
            {
                jobRun.Id = Guid.NewGuid().ToString();
            } while (IsRunningJobIdAvailable(jobRun.Id) == false);

            RequestData agentRequestData = new RequestData
            {
                RequestType = RequestType.RunJob,
                Data = jobRun
            };

            string json = JsonConvert.SerializeObject(agentRequestData);
            byte[] data = Encoding.UTF8.GetBytes(json);

            foundedAgent.RunJob(data);

            RunningJob runningJob = new RunningJob();
            runningJob.JobRun = jobRun;
            runningJob.Id = jobRun.Id;
            runningJob.RunningOnAgent = foundedAgent;

            AddRuningJob(runningJob);

            return "job started";
        }
    }
}
