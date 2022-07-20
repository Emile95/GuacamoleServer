using GuacamoleAgent.ServerApplication;
using Library.Agent;
using Newtonsoft.Json;

string agentDefinitionStr = File.ReadAllText("agent-definition.json");

AgentDefinition agentDefinition = JsonConvert.DeserializeObject<AgentDefinition>(agentDefinitionStr);

ServerSocketHandler serverApplicationHandler = new ServerSocketHandler(1100, agentDefinition);

serverApplicationHandler.Start();

string? line;
do
{
    line = Console.ReadLine();
}
while (line != "quit");