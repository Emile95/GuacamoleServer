namespace Library
{
    public class ServerAgentApplicationLoaded
    {
        public string Id { get; set; }
        public string FilePath { get; set; }
        public byte[] FileBinary { get; set;}
        public List<string> ActionIds { get; set; }
    }
}
