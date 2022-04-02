using System;
using System.Text.Json;

namespace ChatServer.Model
{
    public abstract class ChatBase
    {
        public static string EOF = "<EOF>";

        protected static string ClearFromEOF(string message)
            => message?.Replace(EOF, string.Empty);
    }

    public enum Command
    {
        LOGIN,
        LOGOUT,
        TEXT,
        COUNT,
        ALL_MESSAGE
    }

    public static class ChatResponseFactory
    {
        public static ChatResponse Ack()
            => new ChatResponse("201", false, "ACK");

        public static ChatResponse LoginSucceeded(string IdentityId)
            => new ChatResponse("200", false, IdentityId);

        public static ChatResponse NickNameNotValid(string Nickname)
            => new ChatResponse("400", true, "NickNameNotValid: " + Nickname);

        public static ChatResponse ConnectionCount(int count)
            => new ChatResponse("200", false, "" + count);

        public static ChatResponse SimpleMessage(Message message)
            => new ChatResponse("200", false, message.ToString());
    }


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

    public class ChatCommand : ChatBase
    {
        public static ChatCommand Parse(string command)
        {
            try
            {
                return JsonSerializer.Deserialize<ChatCommand>(ClearFromEOF(command));
            }
            catch (Exception)
            {
                throw new ArgumentException("Unable to parse command: " + command);
            }
        }

        public Command Command { get; set; }
        public string Data { get; set; }
        public User Identity { get; set; }
    }

}
