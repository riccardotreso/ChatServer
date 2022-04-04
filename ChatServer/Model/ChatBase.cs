namespace ChatServer.Model
{
    public abstract class ChatBase
    {
        public static string EOF = "<EOF>";

        protected static string ClearFromEOF(string message)
            => message?.Replace(EOF, string.Empty);
    }

}
