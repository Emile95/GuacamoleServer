using Common;
using Common.Request;
using System.Net.Sockets;

namespace Agent.ServerApplication
{
    public class ServerClient
    {
        private Socket _socket;

        public ServerClient(Socket socket)
        {
            _socket = socket;
        }

        public void FinishAction(string agentActionId)
        {
            byte[] bytes = RequestDataBytesBuilder.BuildRequestDataBytes(ApplicationConstValue.AGENTACTIONFINISHQUESTID, agentActionId);
            _socket.Send(bytes);
        }
    }
}
