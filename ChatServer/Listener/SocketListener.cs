using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ChatServer.Listener
{

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

}
