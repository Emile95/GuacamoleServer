using GuacamoleAgent.Action;
using GuacamoleAgent.ServerApplication.Request;
using Library;
using Library.Agent.Request;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GuacamoleAgent.ServerApplication
{
    public class ServerSocketHandler
    {
        private readonly int _port;
        private readonly Socket _socket;
        private readonly AgentDefinition _agentDefinition;
        private readonly RequestReceivedHandler _requestReceivedHandler;
        private readonly AgentActionManager _agentActionManager;

        public ServerSocketHandler(int port, AgentDefinition agentDefinition)
        {
            _port = port;
            _socket = new Socket(
                Dns.GetHostAddresses(Dns.GetHostName())[0].AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp
            );
            _agentDefinition = agentDefinition;
            _requestReceivedHandler = new RequestReceivedHandler();
            _agentActionManager = new AgentActionManager();
        }

        public void Start()
        {
            AgentRequest data = new AgentRequest
            {
                RequestId = ApplicationConstValue.CONNECTAGENTREQUESTID,
                Data = _agentDefinition
            };

            _socket.Connect(new IPEndPoint(Dns.GetHostAddresses(Dns.GetHostName())[0], _port));

            _socket.Send(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(data)));

            StateObject state = new StateObject();
            state.workSocket = _socket;

            _socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallBack), state);
        }

        private void ReadCallBack(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket socket = state.workSocket;

            int bytesRead = 0;
            try
            {
                bytesRead = socket.EndReceive(ar);
            }
            catch (Exception e)
            {
                return;
            }

            RequestReceivedContext context = new RequestReceivedContext();
            context.Data = state.buffer;
            context.ServerSocket = socket;
            context.AgentActionManager = _agentActionManager;

            _requestReceivedHandler.ProcessRequest(context);

            state.ClearBuffer();

            socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallBack), state);
        }
    }
}
