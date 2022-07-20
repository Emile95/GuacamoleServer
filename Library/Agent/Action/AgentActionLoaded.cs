namespace Library.Agent.Action
{
    public class AgentActionLoaded<ActionInstanceType>
    {
        public string DisplayName { get; set; }
        public string ActionId { get; set; }
        public ActionInstanceType Instance { get; set; }
    }
}
