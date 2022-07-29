using API.AgentAction;
using Common;
using Common.Request;

namespace Agent.ServerApplication
{
    public class ServerOperations
    {
        private Action<byte[]> _sendAction;
        
        public void BindSendAction(Action<byte[]> sendAction)
        {
            _sendAction = sendAction;
        }

        public void FinishRunningAgentAction(string runningActionId, string message)
        {
            RunningAgentActionLog(RunningAgentActionLogType.Succeed, message, runningActionId);
        }
        public void LogErrorRunningAgentAction(string runningActionId, string message)
        {
            RunningAgentActionLog(RunningAgentActionLogType.Error, message, runningActionId);
        }

        public void LogWarningRunningAgentAction(string runningActionId, string message)
        {
            RunningAgentActionLog(RunningAgentActionLogType.Warning, message, runningActionId);
        }

        public void LogInfoRunningAgentAction(string runningActionId, string message)
        {
            RunningAgentActionLog(RunningAgentActionLogType.Info, message, runningActionId);
        }

        public void FatalRunningAgentAction(string runningActionId, string message)
        {
            RunningAgentActionLog(RunningAgentActionLogType.Fatal, message, runningActionId);
        }

        private void RunningAgentActionLog(RunningAgentActionLogType runningAgentActionLogType, string message, string runningAgentActionId)
        {
            RunningAgentActionLog log = new RunningAgentActionLog
            {
                RunningAgentActionId = runningAgentActionId,
                Message = message,
                RunningAgentActionLogType = runningAgentActionLogType
            };
            byte[] bytes = SocketRequestDataBytesBuilder.BuildRequestDataBytes(ApplicationConstValue.AGENTACTIONLOGREQUESTID, log);
            _sendAction(bytes);
        }
    }
}
