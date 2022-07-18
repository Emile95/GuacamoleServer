namespace Application
{
    public static class ApplicationFolderStructure
    {
        public readonly static string AGENTAPPSPATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "agentApps") ;
        public readonly static string SERVERAPPSPATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "serverApps");
    }
}
