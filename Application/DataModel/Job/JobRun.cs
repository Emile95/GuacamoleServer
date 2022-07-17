namespace Application.DataModel.Job
{
    [Serializable]
    public class JobRun
    {
        public string AgentLabel { get; set; }
        public string Script { get; set; }
    }
}
