namespace Library.Agent.Request.DataModel
{
    [Serializable]
    public class JobStartDataModel
    {
        public string Id { get; set; }
        public string AgentLabel { get; set; }
        public string Script { get; set; }
    }
}
