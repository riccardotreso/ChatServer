using System.Text.Json;

namespace ChatServer.Model
{
    public class ChatResponse : ChatBase
    {
        public string Code { get; private set; }
        public bool IsError { get; private set; }
        public string Data { get; private set; }

        public ChatResponse(string code, bool isError, string data)
        {
            Code = code;
            IsError = isError;
            Data = data;
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this) + EOF;
        }
    }

}
