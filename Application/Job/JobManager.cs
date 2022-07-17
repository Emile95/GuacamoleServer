namespace Application.Job
{
    public class JobManager
    {
        private Application.Logger.ILogger _logger;

        private Dictionary<string, RunningJob> _runningJobs;

        public JobManager(Application.Logger.ILogger logger)
        {
            _logger = logger;
            _runningJobs = new Dictionary<string, RunningJob>();
        }

        public void AddRuningJob(RunningJob runningJob)
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

        public bool IsRunningJobIdAvailable(string id)
        {
            return _runningJobs.ContainsKey(id) == false;
        }
    }
}
