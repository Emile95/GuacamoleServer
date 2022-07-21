using Application.Config;

try
{
    ServerConfig serverConfig = ServerConfigResolver.ResolveConfig();

    var serverInstance = new ServerInstance(serverConfig);

    serverInstance.LoadApplications();

    serverInstance.StartSockets();

    serverInstance.RunWebApp(args);

    string? line;
    do
    {
        line = Console.ReadLine();
    }
    while (line != "quit");

    serverInstance.StopSockets();

    serverInstance.StopWebApp();

} catch (Exception ex)
{
    Console.Error.WriteLine(ex.Message);
}


 