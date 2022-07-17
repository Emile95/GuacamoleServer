namespace Application.Agent.Request.DataModel
{
    [Serializable]
    public class JobRunDataModel
    {
        public string Id { get; set; }
        public string AgentLabel { get; set; }
        public string Script { get; set; }
    }
}
