using API.Agent;
using API.Logging;
using Common;
using Common.Request;
using System.Net.Sockets;

namespace Server.Agent
{
    public class AgentClient
    {
        private readonly AgentDefinition _agentDefinition;
        private readonly Socket _socket;
        private readonly AgentLoggers _agentLoggers;
        private bool _locked;
        private int _actionRunning;
        private readonly Action<byte[]> _sendToSocketAction;
        public AgentClient(AgentDefinition agentDefinition, AgentLoggers agentLoggers, Action<byte[]> sendToSocketAction)
        {
            _agentDefinition = agentDefinition;
            _agentLoggers = agentLoggers;
            _actionRunning = 0;
            _sendToSocketAction = sendToSocketAction;
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

        public void InstallAgentApplications(List<AgentApplicationLoaded> agentApplicationLoadeds)
        {
            byte[] bytes = SocketRequestDataBytesBuilder.BuildRequestDataBytes(ApplicationConstValue.INSTALLMODULERAGENTREQUESTID, agentApplicationLoadeds);
            _sendToSocketAction(bytes);
        }

        public void ProcessAction(string actionId, string runningActionId, object parameter)
        {
            byte[] bytes = SocketRequestDataBytesBuilder.BuildRequestDataBytes(actionId, new Tuple<string, object>(runningActionId, parameter));
            _sendToSocketAction(bytes);
        }
    }
}
