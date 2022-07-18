namespace Library.Agent.Request.DataModel
{
    public class JobActionDataModel
    {
        public string JobActionId { get; set; }
        public RequestType RequestType { get; set; }
        public object Data { get; set; }
        public bool Last { get; set; }
    }
}
