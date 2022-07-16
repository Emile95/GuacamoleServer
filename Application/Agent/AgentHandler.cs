using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Application.Agent
{
    public abstract class AgentHandler
    {
        private readonly Application.Logger.ILogger _logger;
        private readonly AgentManager _agentManager;

        protected readonly IPAddress _hostIpAddress;
        protected readonly int _port;
        protected readonly Socket _socket;

        public AgentHandler(Application.Logger.ILogger logger, int port)
        {
            _port = port;
            _hostIpAddress = Dns.GetHostAddresses(Dns.GetHostName())[0];
            _socket = GetSocket();
            _socket.Bind(GetEndpoint());
            _socket.Listen(100);
            _logger = logger;
            _agentManager = new AgentManager();
        }

        public void Start()
        {
            _socket.BeginAccept(new AsyncCallback(AcceptCallBack), _socket);
        }

        public void Stop()
        {
            _socket.Close();
        }

        private void AcceptCallBack(IAsyncResult ar)
        {
            Socket serverSocket = (Socket)ar.AsyncState;
            Socket clientSocket = serverSocket.EndAccept(ar);

            _agentManager.AddAgent(clientSocket);

            //_logger.Log("New Client connected");

            StateObject state = new StateObject();
            state.workSocket = clientSocket;

            clientSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallBack), state);

            serverSocket.BeginAccept(new AsyncCallback(AcceptCallBack), _socket);
        }

        private void ReadCallBack(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket clientSocket = state.workSocket;

            int bytesRead = 0;
            try
            {
                bytesRead  = clientSocket.EndReceive(ar);
            }
            catch (Exception e)
            {
                _logger.Log("Client Lost unexpectly");
                _agentManager.RemoveAgent(clientSocket);
                return;
            }

            ServerAgentData data = JsonConvert.DeserializeObject<ServerAgentData>(Encoding.ASCII.GetString(state.buffer));

            JObject jObject = (JObject)data.Data;

            switch (data.CommunicationType)
            {
                case "agentAuth":
                    AgentDefinition agentDefinition = jObject.ToObject<AgentDefinition>();
                    Console.WriteLine("Agent " + agentDefinition.label + " connected"); 
                break;
            }

            clientSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallBack), state);
        }

        protected abstract Socket GetSocket();

        protected abstract EndPoint GetEndpoint();
    }
}
