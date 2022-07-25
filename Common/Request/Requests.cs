namespace Common.Request
{
    public class Requests
    {
        public bool IsRequestBeingSend { get; set; }
        public List<byte[]> PendingRequestDatas { get; set; }

        public Requests()
        {
            PendingRequestDatas = new List<byte[]>();
        }
    }
}
