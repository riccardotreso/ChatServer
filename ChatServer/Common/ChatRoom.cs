using System;
using System.Collections.Generic;
using System.Net.Sockets;
using ChatServer.Model;
using System.Linq;

namespace ChatServer
{

    public class ClientConnected {
        public User Identity { get; set; }
        public Socket Connection { get; set; }
    }

    public class ChatRoom
    {
        List<ClientConnected> clients;

        public ChatRoom()
        {
            clients = new List<ClientConnected>();
        }

        public ChatResponse GetChatResponseFromCommand(ChatCommand chatCommand, Socket socket) {
            if (chatCommand.Command == Command.LOGIN) {
                string nickname = chatCommand.Identity.NickName;
                if (NickNameNotExists(nickname)) {
                    var identity = AddClient(nickname, socket);
                    return ChatResponseFactory.LoginSucceeded(identity.Id);
                }
                return ChatResponseFactory.NickNameNotValid(chatCommand.Identity.NickName);
            }
            return null;
            
        }

        private User AddClient(string NickName, Socket socket) {
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
