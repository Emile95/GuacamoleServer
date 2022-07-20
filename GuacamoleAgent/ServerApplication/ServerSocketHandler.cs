using GuacamoleAgent.Action;
using GuacamoleAgent.ServerApplication.Request;
using Library;
using Library.Agent;
using Library.Agent.Request;
using Library.Sockets;
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
        private readonly ClientAgentActionManager _agentActionManager;

        public ServerSocketHandler(int port, string protocol, AgentDefinition agentDefinition)
        {
            _port = port;

            if(protocol == "tcp")
            {
                _socket = new Socket(
                    Dns.GetHostAddresses(Dns.GetHostName())[0].AddressFamily,
                    SocketType.Stream,
                    ProtocolType.Tcp
                );
            }
            _agentDefinition = agentDefinition;
            _requestReceivedHandler = new RequestReceivedHandler();
            _agentActionManager = new ClientAgentActionManager();
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
            context.ServerSocket = socket;
            context.AgentActionManager = _agentActionManager;

            context.Data = new byte[bytesRead];
            for (int i = 0; i < bytesRead; i++)
                context.Data[i] = state.buffer[i];

            _requestReceivedHandler.ProcessRequest(context);

            state.ClearBuffer();

            socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallBack), state);
        }
    }
}
