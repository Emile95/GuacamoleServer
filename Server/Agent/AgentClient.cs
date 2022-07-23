using API.Agent;
using Common;
using Common.Request;
using System.Net.Sockets;

namespace Server.Agent
{
    public class AgentClient
    {
        private readonly AgentDefinition _agentDefinition;
        private readonly Socket _socket;
        private readonly API.Logger.ILogger _logger;
        private bool _locked;
        private int _actionRunning;

        public AgentClient(AgentDefinition agentDefinition, Socket socket, API.Logger.ILogger logger)
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

        public void InstallAgentApplication(AgentApplicationLoaded agentApplicationLoaded)
        {
            _socket.Send(SocketRequestDataBytesBuilder.BuildRequestDataBytes(ApplicationConstValue.INSTALLMODULERAGENTREQUESTID, agentApplicationLoaded));
        }

        public void ProcessAction(string actionId, string runningActionId)
        {
            _socket.Send(SocketRequestDataBytesBuilder.BuildRequestDataBytes(actionId, runningActionId));
        }
    }
}
