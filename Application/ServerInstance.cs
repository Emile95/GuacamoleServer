﻿using Library.EventHandler;
using Library.Application;
using Application.Agent;
using Application.Logger;

public class ServerInstance
{
    private readonly ApplicationManager _applicationManager;
    private readonly ApplicationResolver _applicationResolver;
    private readonly EventHandlerManager _eventHandlerManager;
    private WebApplication _webApplication;

    private Application.Logger.ILogger _logger;

    private readonly TCPAgentHandler _tcpAgentHandler;

    public ServerInstance()
    {
        _eventHandlerManager = new EventHandlerManager();
        _applicationResolver = new ApplicationResolver(_eventHandlerManager);
        _applicationManager = new ApplicationManager(
            _applicationResolver,
            _eventHandlerManager
        );

        _logger = new ConsoleLogger();

        _tcpAgentHandler = new TCPAgentHandler(_logger, 1100);
    }

    public void LoadApplications()
    {
        _applicationManager.LoadApplications();
    }

    public void RunWebApp(string[] args)
    {
        _webApplication = Application.RestAPI.WebApplicationBuilder.BuildWebApplication(_applicationManager, _tcpAgentHandler);
        _webApplication.RunAsync();
    }

    public void StopWebApp()
    {
        _webApplication.StopAsync();
    }

    public void StartSockets()
    {
        _tcpAgentHandler.Start();
    }

    public void StopSockets()
    {
        _tcpAgentHandler.Stop();
    }
}

