namespace Library
{
    public static class ApplicationConstValue
    {
        public readonly static string AGENTAPPSPATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "agentApps");
        public readonly static string SERVERAPPSPATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "serverApps");

        public readonly static string INSTALLMODULERAGENTREQUESTID = "InstallModule";
        public readonly static string CONNECTAGENTREQUESTID = "AgentConnect";
        public readonly static string SERVERCONFIGFILEPATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
    }
}
