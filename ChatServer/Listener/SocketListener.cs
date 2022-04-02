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
        public void Initialize();
        public EventHandler<ChatCommand> OnMessageArrived { get; set; }
    }

    public class SocketChat : ISocketServer
    {
        private string EOF = "<EOF>";
        public EventHandler<ChatCommand> OnMessageArrived { get; set; }

        public void Initialize()
        {
            Task.Run(() =>
            {
                // Incoming data from the client.  
                string data = null;

                // Data buffer for incoming data.  
                byte[] bytes = new Byte[1024];

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
                        data = null;

                        // An incoming connection needs to be processed.  
                        while (true)
                        {
                            int bytesRec = handler.Receive(bytes);
                            data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                            if (data.IndexOf(EOF) > -1)
                            {
                                break;
                            }
                        }

                        if (OnMessageArrived != null) {
                            ChatCommand command = GetChatCommandFromDataReceived(data);
                            OnMessageArrived(this, command);
                        }

                        // Show the data on the console.  
                        Console.WriteLine("Text received : {0}", data);

                        // Echo the data back to the client.  
                        byte[] msg = Encoding.ASCII.GetBytes(data);

                        handler.Send(msg);
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                Console.WriteLine("\nPress ENTER to continue...");
                Console.Read();


            });

        }
        private ChatCommand GetChatCommandFromDataReceived(string data)
        {
            data = data.Replace(EOF, string.Empty);
            return ChatCommand.Parse(data);
        }
    }

}
