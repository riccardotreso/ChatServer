using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ChatServer.Model;

namespace ChatServer.Listener
{
    public static class ClientListener
    {
        public static void Inizialize(Socket handler, ChatRoom room)
        {
            Task.Run(() =>
            {
                Guid ClientId = Guid.NewGuid();
                // Data buffer for incoming data.  
                byte[] bytes = new byte[1024];
                // Incoming data from the client.  

                while (handler.Connected)
                {
                    // An incoming connection needs to be processed.
                    string data = null;
                    while (handler.Connected)
                    {
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf(ChatBase.EOF) > -1)
                        {
                            break;
                        }

                        if (data.Length == 0)
                        {
                            handler.Shutdown(SocketShutdown.Both);
                            handler.Close();
                            room.Logout(ClientId);
                            break;
                        }
                    }

                    if (data.Length > 0)
                    {
                        ChatCommand command = GetChatCommandFromDataReceived(data);

                        var response = room.GetChatResponseFromCommand(command, handler, ClientId);

                        byte[] msg = Encoding.ASCII.GetBytes(response.ToString());

                        handler.Send(msg);
                    }
                }
            });
        }

        private static ChatCommand GetChatCommandFromDataReceived(string data)
        {
            return ChatCommand.Parse(data);
        }

    }

}
