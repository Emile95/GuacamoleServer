using Application.Agent;
using Library.Agent;
using Library.Agent.Request.DataModel;
using Application.DataModel;
using Library.Agent.Request;

namespace Application.Job
{
    public class JobManager
    {
        private Library.Logger.ILogger _logger;

        private readonly AgentManager _agentManager;
        private readonly JobStorage _jobStorage;

        private Dictionary<string, RunningJob> _runningJobs;

        public JobManager(Library.Logger.ILogger logger, AgentManager agentManager, JobStorage jobStorage)
        {
            _logger = logger;
            _agentManager = agentManager;
            _jobStorage = jobStorage;
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

        public object StartJobOnAgent(StartJobDataModel model)
        {
            AgentClient foundedAgent = null;
            try
            {
                foundedAgent = _agentManager.GetAvailableAgentByLabel(model.AgentLabel);
            }
            catch (Exception e) { return e.Message; }

            JobStartDataModel jobRun = CreateJobRunDataModel(model);

            foundedAgent.StartJob(jobRun);

            RunningJob runningJob = CreateRunningJob(jobRun, foundedAgent);

            AddRuningJob(runningJob);

            return "job started";
        }

        private JobStartDataModel CreateJobRunDataModel(StartJobDataModel model)
        {
            JobStartDataModel jobRun = new JobStartDataModel();
            jobRun.AgentLabel = model.AgentLabel;
            jobRun.Script = "adsdasdasd";
            jobRun.Id = GenerateRunningJobId();
            return jobRun;
        }

        private string GenerateRunningJobId()
        {
            string id = "";
            do
            {
                id = Guid.NewGuid().ToString();
            } while (IsRunningJobIdAvailable(id) == false);
            return id;
        }

        private RunningJob CreateRunningJob(JobStartDataModel model, AgentClient agentClient)
        {
            RunningJob runningJob = new RunningJob();
            runningJob.JobRun = model;
            runningJob.Id = model.Id;
            runningJob.RunningOnAgent = agentClient;
            return runningJob;
        }
    }
}
