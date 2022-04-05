using System;
using System.Collections.Generic;
using System.Net.Sockets;
using ChatServer.Model;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace ChatServer
{

    public class ClientConnected
    {
        public User Identity { get; set; }
        public Socket Connection { get; set; }
        public Guid ClientId { get; set; }
    }

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

    public class ChatRoom
    {
        List<ClientConnected> clients;
        List<Message> messages;

        public int GetCountMessageFromUserId(string id)
        {
            string nickName = clients.FirstOrDefault(x => x.Identity.Id.Equals(id))?.Identity.NickName ?? string.Empty;
            if (!string.IsNullOrEmpty(nickName))
                return messages.Count(x => x.NickName.Equals(nickName));
            return -1;
        }

        public ChatRoom()
        {
            clients = new List<ClientConnected>();
            messages = new List<Message>();
        }
        public List<KeyValuePair<string, string>> GetClientConnected()
            => clients.ToDictionary(k => k.Identity.Id, v => v.Identity.NickName).ToList();

        public void Logout(Guid clientId)
        {
            var clientToDelete = clients.FirstOrDefault(x => x.ClientId.Equals(clientId));
            if (clientToDelete != null)
                clients.Remove(clientToDelete);
        }
        public ChatResponse GetChatResponseFromCommand(ChatCommand chatCommand, Socket socket, Guid clientId)
        {

            var response = chatCommand.Command switch
            {
                Command.LOGIN => Login(chatCommand, socket, clientId),
                Command.TEXT => TextAndPropagate(chatCommand),
                _ => null
            };

            return response;
        }
        private ChatResponse Login(ChatCommand chatCommand, Socket socket, Guid clientId)
        {
            string nickname = chatCommand.Identity.NickName;
            if (NickNameNotExists(nickname))
            {
                var identity = AddClient(nickname, socket, clientId);
                return ChatResponseFactory.LoginSucceeded(identity.Id);
            }
            return ChatResponseFactory.NickNameNotValid(chatCommand.Identity.NickName);
        }

        private ChatResponse TextAndPropagate(ChatCommand chatCommand)
        {
            var message = new Message { Text = chatCommand.Data, NickName = chatCommand.Identity.NickName };
            
            AddMessageAndPropagate(message);

            return ChatResponseFactory.Ack();
        }

        public void AddMessageAndPropagate(Message message)
        {
            messages.Add(message);
            Propagate(message);
        }

        private void Propagate(Message messageToPropagate)
        {
            Task.Run(() =>
            {
                byte[] msg = Encoding.ASCII.GetBytes(ChatResponseFactory.SimpleMessage(messageToPropagate).ToString());

                _ = clients.Select(c => c.Connection?.Send(msg)).ToList();
            });
        }

        private User AddClient(string NickName, Socket socket, Guid clientId)
        {
            User Identity = new User
            {
                Id = (clients.Count + 1).ToString(),
                NickName = NickName
            };
            clients.Add(new ClientConnected { Connection = socket, Identity = Identity, ClientId = clientId });
            return Identity;
        }
        private bool NickNameNotExists(string NickName)
            => !clients.Any(c => c.Identity.NickName.Equals(NickName));

        public List<string> GetAllMessages()
            => messages
                .OrderBy(x=> x.Time)
                .Select(x=> x.ToString())
                .ToList();
    }
}
