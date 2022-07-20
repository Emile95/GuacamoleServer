namespace GuacamoleAgent.DataModel
{
    [Serializable]
    public class JobRun
    {
        public string Id { get; set; }
        public string AgentLabel { get; set; }
        public string Script { get; set; }
    }
}
