using Agent;

Configuration.ResolveConfig();

AgentInstance agentInstance = new AgentInstance();

agentInstance.StartSocket();

string line;
do
{
    line = Console.ReadLine();
}
while (line != "quit");