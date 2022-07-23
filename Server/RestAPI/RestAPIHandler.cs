﻿using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Sockets;

namespace Server.RestAPI
{
    public class RestAPIHandler
    {
        private readonly API.Logger.ILogger _logger;
        private WebApplication _webApp;
        private readonly Dictionary<HttpRequestMethod, Dictionary<string,Delegate>> _mappedRequests;

        public RestAPIHandler(API.Logger.ILogger logger)
        {
            _logger = logger;
            _mappedRequests = new Dictionary<HttpRequestMethod, Dictionary<string, Delegate>>();
            _mappedRequests.Add(HttpRequestMethod.POST, new Dictionary<string, Delegate>());
            _mappedRequests.Add(HttpRequestMethod.GET, new Dictionary<string, Delegate>());
        }

        public void MapPost<Body>(string pattern, Func<Body,object> action)
        {
            _mappedRequests[HttpRequestMethod.POST].Add(pattern, ([FromBody] Body body) =>
            {
                try
                {
                    return action(body);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            });

            _logger.Log("Http POST request mapped, pattern : '" + pattern + "'");
        }

        public void MapGet(string pattern, Func<object> action)
        {
            _mappedRequests[HttpRequestMethod.GET].Add(pattern, () =>
            {
                try
                {
                    return action();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            });

            _logger.Log("Http GET request mapped, pattern : '" + pattern + "'");
        }

        public void Run()
        {
            Build();
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    string ipStr = "http://"+ ip.ToString() + ":5000";
                    _webApp.RunAsync(ipStr);
                }
            }
        }

        public void Restart()
        {
            _webApp.StopAsync();
            Run();
        }

        public void Stop()
        {
            _webApp.StopAsync();
        }

        private void Build()
        {
            var builder = WebApplication.CreateBuilder();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    name: "prelude",
                    builder => builder.WithOrigins("*")
                );
            });

            _webApp = builder.Build();

            foreach(KeyValuePair<HttpRequestMethod, Dictionary<string, Delegate>> mappedRequest in _mappedRequests)
            {
                switch(mappedRequest.Key)
                {
                    case HttpRequestMethod.POST:
                        foreach(KeyValuePair<string, Delegate> postRquest in mappedRequest.Value)
                            _webApp.MapPost(postRquest.Key, postRquest.Value);
                        break;
                    case HttpRequestMethod.GET:
                        foreach (KeyValuePair<string, Delegate> getRequest in mappedRequest.Value)
                            _webApp.MapGet(getRequest.Key, getRequest.Value);
                        break;
                }
            }
        }
    }
}
