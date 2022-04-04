namespace ChatServer.Model
{
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

}
