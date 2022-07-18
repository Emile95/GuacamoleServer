using Library.Agent.Request;
using Library.Agent.Request.DataModel;
using System.Net.Sockets;

namespace Library.Agent
{
    public class AgentClient
    {
        private readonly AgentDefinition _agentDefinition;
        private readonly Socket _socket;
        private readonly Library.Logger.ILogger _logger;
        private bool _locked;
        private int _jobrunning;

        public AgentClient(AgentDefinition agentDefinition, Socket socket, Library.Logger.ILogger logger)
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

        public void TransferFile(byte[] file, string path, string jobActionId = null)
        {
            TransferFileDataModel transferFileDataModel = new TransferFileDataModel
            {
                File = file,
                Path = path
            };
            SendRequest(RequestType.TransferFile, transferFileDataModel);
        }

        public void StartJob(JobStartDataModel model)
        {
            _jobrunning++;
            SendRequest(RequestType.StartJob, model);
        }

        private void SendRequest(RequestType requestType, object data, string jobActionId = null)
        {
            object dataToSend = data;
            if (jobActionId != null)
            {
                JobActionDataModel jobActionDataModel = new JobActionDataModel
                {
                    JobActionId = jobActionId,
                    RequestType = requestType,
                    Data = data
                };
                dataToSend = jobActionDataModel;
            }
            byte[] bytes = RequestDataBytesBuilder.BuildRequestDataBytes(requestType, dataToSend);
            _socket.Send(bytes);
        }
    }
}
