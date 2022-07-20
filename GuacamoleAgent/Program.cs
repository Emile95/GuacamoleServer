using GuacamoleAgent;
using GuacamoleAgent.Config;

AgentConfig config = AgentConfigResolver.ResolveConfig();

AgentInstance agentInstance = new AgentInstance(config);

agentInstance.StartSocket();

string line;
do
{
    line = Console.ReadLine();
}
while (line != "quit");