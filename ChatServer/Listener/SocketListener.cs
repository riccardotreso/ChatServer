using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ChatServer.Model;

namespace ChatServer.Listener
{

    public interface ISocketServer
    {
        public void Initialize(ChatRoom chatRoom);
    }

    public class SocketChat : ISocketServer
    {
        ChatRoom room;
        public void Initialize(ChatRoom chatRoom)
        {
            room = chatRoom;
            Task.Run(() =>
            {
                // Establish the local endpoint for the socket.  
                // Dns.GetHostName returns the name of the
                // host running the application.
                //var hostnamae = Dns.GetHostName();
                IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

                // Create a TCP/IP socket.  
                Socket listener = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Bind the socket to the local endpoint and
                // listen for incoming connections.  
                try
                {
                    listener.Bind(localEndPoint);
                    listener.Listen(10);

                    // Start listening for connections.  
                    while (true)
                    {
                        Console.WriteLine("Waiting for a connection...");
                        // Program is suspended while waiting for an incoming connection.  
                        Socket handler = listener.Accept();

                        ClientListener.Inizialize(handler, room);

                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

            });

        }
        
    }

    public static class ClientListener {
        public static void Inizialize(Socket handler, ChatRoom room) {
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
