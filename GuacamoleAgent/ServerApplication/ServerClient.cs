using Library;
using Library.Agent.Request;
using System.Net.Sockets;

namespace GuacamoleAgent.ServerApplication
{
    public class ServerClient
    {
        private Socket _socket;

        public ServerClient(Socket socket)
        {
            _socket = socket;
        }

        public void FinishAction()
        {
            byte[] bytes = RequestDataBytesBuilder.BuildRequestDataBytes(ApplicationConstValue.AGENTACTIONFINISHQUESTID, null);
            _socket.Send(bytes);
        }
    }
}
