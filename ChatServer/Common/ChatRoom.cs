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
    }

    public class Message
    {
        public string NickName { get; set; }
        public string Text { get; set; }

        public override string ToString()
        {
            return $"{NickName} says >> {Text}";
        }
    }

    public class ChatRoom
    {
        List<ClientConnected> clients;
        List<Message> messages;

        public ChatRoom()
        {
            clients = new List<ClientConnected>();
            messages = new List<Message>();
        }

        public ChatResponse GetChatResponseFromCommand(ChatCommand chatCommand, Socket socket)
        {

            var response = chatCommand.Command switch
            {
                Command.LOGIN => Login(chatCommand, socket),
                Command.TEXT => TextAndPropagate(chatCommand),
                _ => null
            };

            return response;
        }
        private ChatResponse Login(ChatCommand chatCommand, Socket socket)
        {
            string nickname = chatCommand.Identity.NickName;
            if (NickNameNotExists(nickname))
            {
                var identity = AddClient(nickname, socket);
                return ChatResponseFactory.LoginSucceeded(identity.Id);
            }
            return ChatResponseFactory.NickNameNotValid(chatCommand.Identity.NickName);
        }

        private ChatResponse TextAndPropagate(ChatCommand chatCommand)
        {
            var message = new Message { Text = chatCommand.Data, NickName = chatCommand.Identity.NickName };
            messages.Add(message);

            Propagate(message);

            return ChatResponseFactory.Ack();
        }

        private void Propagate(Message messageToPropagate)
        {
            Task.Run(() =>
            {
                byte[] msg = Encoding.ASCII.GetBytes(ChatResponseFactory.SimpleMessage(messageToPropagate).ToString());

                _ = clients.Select(c => c.Connection.Send(msg)).ToList();
            });
        }

        private User AddClient(string NickName, Socket socket)
        {
            User Identity = new User
            {
                Id = (clients.Count + 1).ToString(),
                NickName = NickName
            };
            clients.Add(new ClientConnected { Connection = socket, Identity = Identity });
            return Identity;
        }
        private bool NickNameNotExists(string NickName)
            => !clients.Any(c => c.Identity.NickName.Equals(NickName));


    }
}
