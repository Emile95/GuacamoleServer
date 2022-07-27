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
        private Requests _pendingRequests;

        public AgentClient(AgentDefinition agentDefinition, Socket socket, AgentLoggers agentLoggers)
        {
            _agentDefinition = agentDefinition;
            _socket = socket;
            _agentLoggers = agentLoggers;
            _actionRunning = 0;
            _pendingRequests = new Requests();
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

        public void InstallAgentApplications(List<AgentApplicationLoaded> agentApplicationLoadeds)
        {
            SendRequest(SocketRequestDataBytesBuilder.BuildRequestDataBytes(ApplicationConstValue.INSTALLMODULERAGENTREQUESTID, agentApplicationLoadeds));
        }

        public void ProcessAction(string actionId, string runningActionId, object parameter)
        {
            SendRequest(SocketRequestDataBytesBuilder.BuildRequestDataBytes(actionId, new Tuple<string, object>(runningActionId, parameter)));
        }

        public void LiberateRequestSend()
        {
            if(_pendingRequests.PendingRequestDatas.Count > 0)
            {
                _socket.Send(_pendingRequests.PendingRequestDatas[0]);
                _pendingRequests.PendingRequestDatas.RemoveAt(0);
                return;
            }
            _pendingRequests.IsRequestBeingSend = false;
        }

        private void SendRequest(byte[] bytes)
        {
            if(_pendingRequests.IsRequestBeingSend == false)
            {
                _pendingRequests.IsRequestBeingSend = true;
                _socket.Send(bytes);
                return;
            }
            _pendingRequests.PendingRequestDatas.Add(bytes);
        }
    }
}
