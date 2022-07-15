var serverInstance = new ServerInstance();

serverInstance.RunWebApp(args);

string? line;
do
{
    line = Console.ReadLine();
}
while (line != "quit");

serverInstance.StopWebApp();