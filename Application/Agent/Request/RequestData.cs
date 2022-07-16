namespace Application.Agent.Request
{
    public class RequestData
    {
        public string Id { get; set; }
        public RequestType RequestType { get; set; }
        public object Data { get; set; }
    }
}
