using System.Net.Sockets;

namespace Application.Agent
{
    public class AgentClient
    {
        private readonly AgentDefinition _agentDefinition;
        private readonly Socket _socket;
        private readonly Application.Logger.ILogger _logger;
        private bool _locked;
        private int _jobrunning;

        public AgentClient(AgentDefinition agentDefinition, Socket socket, Application.Logger.ILogger logger)
        {
            _agentDefinition = agentDefinition;
            _socket = socket;
            _logger = logger;
            _jobrunning = 0;
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

        public void DecrementJobRunning()
        {
            _jobrunning--;
        }

        public bool IsAvailable()
        {
            return _locked == false && _jobrunning < _agentDefinition.ConcurrentRun;
        }

        public void RunJob(byte[] jobData)
        {
            _jobrunning++;
            try {
                _socket.Send(jobData);
            } catch (Exception e) {
                _jobrunning--;
                return;
            }
        }
    }
}
