using System;
using System.Text.Json;

namespace ChatServer.Model
{
    public class ChatCommand
    {
        public static ChatCommand Parse(string command)
        {
            try
            {
                return JsonSerializer.Deserialize<ChatCommand>(command);
            }
            catch (Exception) { 
                throw new ArgumentException("Unable to parse command: " + command);
            }
        }

        public string Command { get; set; }
        public string Data { get; set; }
        public User Identity { get; set; }
    }

}
