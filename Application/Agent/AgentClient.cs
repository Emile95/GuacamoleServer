using Application.Agent.Request;
using Application.Agent.Request.DataModel;
using Application.DataModel;
using Application.Job;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Text;

namespace Application.Agent
{
    public class AgentClient
    {
        private readonly AgentDefinition _agentDefinition;
        private readonly Socket _socket;
        private readonly Application.Logger.ILogger _logger;
        private bool _locked;
        private int _jobrunning;
        private readonly JobManager _jobManager;

        public AgentClient(AgentDefinition agentDefinition, Socket socket, Application.Logger.ILogger logger, JobManager jobManager)
        {
            _agentDefinition = agentDefinition;
            _socket = socket;
            _logger = logger;
            _jobrunning = 0;
            _jobManager = jobManager;
        }

        public bool IsEqualBySocket(Socket socket)
        {
            return _socket.Equals(socket);
        }

        public string GetName()
        {
            return _agentDefinition.Name;
        }

        public bool IsLocked()
        {
            return _locked;
        }

        public void Lock()
        {
            _locked = true;
        }

        public void UnLock()
        {
            _locked = false; ;
        }

        public List<string> GetLabels()
        {
            return _agentDefinition.Labels;
        }

        public void AnJobIsFinish()
        {
            _jobrunning--;
        }

        public bool IsAvailable()
        {
            return _locked == false && _jobrunning < _agentDefinition.ConcurrentRun;
        }

        public void RunJob(StartJobDataModel job)
        {
            JobRunDataModel jobRun = new JobRunDataModel();
            jobRun.AgentLabel = job.AgentLabel;
            jobRun.Script = job.Script;

            do
            {
                jobRun.Id = Guid.NewGuid().ToString();
            } while (_jobManager.IsRunningJobIdAvailable(jobRun.Id) == false);

            RequestData agentRequestData = new RequestData
            {
                RequestType = RequestType.RunJob,
                Data = jobRun
            };

            string json = JsonConvert.SerializeObject(agentRequestData);
            byte[] data = Encoding.UTF8.GetBytes(json);

            _jobrunning++;
            try {
                _socket.Send(data);
            } catch (Exception e) {
                _jobrunning--;
                return;
            }

            RunningJob runningJob = new RunningJob();
            runningJob.JobRun = jobRun;
            runningJob.Id = jobRun.Id;
            runningJob.RunningOnAgent = this;

            _jobManager.AddRuningJob(runningJob);
        }
    }
}
