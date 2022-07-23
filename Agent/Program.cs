using Agent;
using Agent.Config;
using System.Diagnostics;

Debugger.Launch();

AgentConfig config = AgentConfigResolver.ResolveConfig();

AgentInstance agentInstance = new AgentInstance(config);

agentInstance.StartSocket();

string line;
do
{
    line = Console.ReadLine();
}
while (line != "quit");