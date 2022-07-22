using Library;
using Library.Agent;
using Library.Agent.Action;
using Library.Agent.Request;
using System.Net.Sockets;

namespace Application.Agent
{
    public class AgentClient
    {
        private readonly AgentDefinition _agentDefinition;
        private readonly Socket _socket;
        private readonly Library.Logger.ILogger _logger;
        private bool _locked;
        private int _actionRunning;

        public AgentClient(AgentDefinition agentDefinition, Socket socket, Library.Logger.ILogger logger)
        {
            _agentDefinition = agentDefinition;
            _socket = socket;
            _logger = logger;
            _actionRunning = 0;
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

        public bool IsAvailable()
        {
            return _locked == false && _actionRunning < _agentDefinition.ConcurrentRun;
        }

        public void InstallAgentAction(AgentActionLoaded<Tuple<string, byte[]>> agentActionLoaded)
        {
            _socket.Send(RequestDataBytesBuilder.BuildRequestDataBytes(ApplicationConstValue.INSTALLMODULERAGENTREQUESTID, agentActionLoaded));
        }

        public void ProcessAction(string actionId, string runningActionId)
        {
            _socket.Send(RequestDataBytesBuilder.BuildRequestDataBytes(actionId, runningActionId));
        }
    }
}
