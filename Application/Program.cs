var serverInstance = new ServerInstance();

serverInstance.LoadApplications();

serverInstance.StartSockets();

serverInstance.RunWebApp(args);

string? line;
do
{
    line = Console.ReadLine();
}
while(line != "quit");

serverInstance.StopSockets();

serverInstance.StopWebApp();