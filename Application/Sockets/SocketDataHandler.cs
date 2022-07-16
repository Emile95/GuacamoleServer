using Newtonsoft.Json;

namespace Application.Sockets
{
    public class SocketDataHandler
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public SocketDataHandler()
        {
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
        }

        public SocketData CreateSocketDataFromBytes(byte[] bytes)
        {
            string data = System.Text.Encoding.ASCII.GetString(bytes);
            return JsonConvert.DeserializeObject<SocketData>(data);
        }

        public byte[] CreateBytesFromSocketData(SocketData socketData)
        {
            string data = JsonConvert.SerializeObject(socketData, _jsonSerializerSettings);
            return System.Text.Encoding.ASCII.GetBytes(data);
        }
    }
}
