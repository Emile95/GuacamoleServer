using GuacamoleAgent.Config;
using GuacamoleAgent.ServerApplication;
using Library.Agent;

namespace GuacamoleAgent
{
    public class AgentInstance
    {
        private readonly AgentConfig _config;
        private readonly AgentDefinition _agentDefinition;

        private readonly ServerSocketHandler _serverApplicationHandler;

        public AgentInstance(AgentConfig config)
        {
            _config = config;

            _agentDefinition = new AgentDefinition
            {
                Id = _config.Id,
                Name = _config.Name,
                Labels = _config.Labels,
                ConcurrentRun = _config.ConcurrentRun
            };

            _serverApplicationHandler = new ServerSocketHandler(_config.ServerSocketConfig.Port, _config.ServerSocketConfig.Protocol, _agentDefinition);
        }

        public void StartSocket()
        {
            _serverApplicationHandler.Start();
        }
    }
}
