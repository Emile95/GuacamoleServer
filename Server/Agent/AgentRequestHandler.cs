﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Server.AgentAction;
using Common.Sockets;
using Server.Application;
using Common;
using Common.Request;
using API.Agent;

namespace Server.Agent
{
    public class AgentRequestHandler
    {
        private readonly API.Logger.ILogger _logger;
        private readonly AgentApplicationManager _agentApplicationManager;
        private readonly AgentActionManager _serverAgentActionManager;
        private readonly AgentManager _agentManager;

        public AgentRequestHandler(API.Logger.ILogger logger, AgentApplicationManager agentApplicationManager, AgentActionManager serverAgentActionManager, AgentManager agentManager)
        {
            _logger = logger;
            _agentApplicationManager = agentApplicationManager;
            _serverAgentActionManager = serverAgentActionManager;
            _agentManager = agentManager;
        }

        public void ProcessRequest(SocketRequestContext context)
        {
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.TypeNameHandling = TypeNameHandling.All;

            SocketRequest agentRequest = JsonConvert.DeserializeObject<SocketRequest>(Encoding.ASCII.GetString(context.Data), setting);

            if (agentRequest.RequestId == ApplicationConstValue.CONNECTAGENTREQUESTID)
            {
                JObject jObject = (JObject)agentRequest.Data;
                ConnectAgent(context, jObject.ToObject<AgentDefinition>());
                return;
            }

            if (agentRequest.RequestId == ApplicationConstValue.AGENTACTIONFINISHQUESTID)
            {
                AgentActionFinish(context, agentRequest.Data as string);
                return;
            }
        }

        private void ConnectAgent(SocketRequestContext context, AgentDefinition agentDefinition)
        {
            AgentClient agentClient = _agentManager.AddAgent(agentDefinition, context.SourceSocket);
            _logger.Log("Agent " + agentDefinition.Name + " connected");

            foreach(AgentApplicationLoaded serverAgentApplicationLoaded in _agentApplicationManager.GetAgentApplicationLoadeds())
                agentClient.InstallAgentApplication(serverAgentApplicationLoaded);
        }

        private void AgentActionFinish(SocketRequestContext context, string runningActionId)
        {
            _serverAgentActionManager.RemoveRunningAction(runningActionId);
            _logger.Log("Request finish, id : " + runningActionId);
        }
    }
}