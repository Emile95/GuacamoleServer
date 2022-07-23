using API.AgentAction;
using Common;
using Common.Request;
using System.Net.Sockets;

namespace API.Server
{
    public class ServerOperations
    {
        private Socket _socket;

        public ServerOperations(Socket socket)
        {
            _socket = socket;
        }

        public void FinishRunningAgentAction(string runningActionId, string message)
        {
            RunningAgentActionLog log = new RunningAgentActionLog
            {
                RunningAgentActionId = runningActionId,
                Message = message,
                RunningAgentActionLogType = RunningAgentActionLogType.Succeed
            };
            RunningAgentActionLog(log);
        }

        private void RunningAgentActionLog(RunningAgentActionLog log)
        {
            byte[] bytes = SocketRequestDataBytesBuilder.BuildRequestDataBytes(ApplicationConstValue.AGENTACTIONLOGREQUESTID, log);
            _socket.Send(bytes);
        }
    }
}
