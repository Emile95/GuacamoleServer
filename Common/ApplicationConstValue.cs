namespace Common
{
    public static class ApplicationConstValue
    {
        // Directory/File Path
        public readonly static string AGENTAPPSPATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "agentApps");
        public readonly static string SERVERAPPSPATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "serverApps");
        public readonly static string CONFIGFILEPATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"config.json");
        public readonly static string LOGDIRECTORYPATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

        public readonly static string HTTPREQUESTLOGFILEPATH = Path.Combine(LOGDIRECTORYPATH, "httprequest.log");
        public readonly static string AGENTLOGFILEPATH = Path.Combine(LOGDIRECTORYPATH, "agent.log");
        public readonly static string AGENTACTIONLOGFILEPATH = Path.Combine(LOGDIRECTORYPATH, "agentAction.log");
        public readonly static string SOCKETLOGFILEPATH = Path.Combine(LOGDIRECTORYPATH, "socket.log");
        public readonly static string SOCKETREQUESTLOGFILEPATH = Path.Combine(LOGDIRECTORYPATH, "socketRequest.log");

        // Agent Requests
        public readonly static string INSTALLMODULERAGENTREQUESTID = "InstallModule";
        public readonly static string CONNECTAGENTREQUESTID = "AgentConnect";
        public readonly static string AGENTACTIONLOGREQUESTID = "AgentActionLog";
    }
}
