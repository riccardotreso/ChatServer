using System;

namespace ChatServer
{
    public class Message
    {
        public string NickName { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; private set; } = DateTime.Now;

        public override string ToString()
        {
            return $"{NickName} says >> {Text}";
        }
    }
}
