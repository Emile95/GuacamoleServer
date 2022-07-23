using Server.Config;

try
{
    ServerConfig serverConfig = ServerConfigResolver.ResolveConfig();

    var serverInstance = new ServerInstance(serverConfig);

    serverInstance.LoadServerApplications();

    serverInstance.LoadAgentApplications();

    serverInstance.StartSockets();

    serverInstance.RunWebApp(args);

    string? line;
    do
    {
        line = Console.ReadLine();
    }
    while (line != "quit");

} catch (Exception ex)
{
    Console.Error.WriteLine(ex.Message);
}


 