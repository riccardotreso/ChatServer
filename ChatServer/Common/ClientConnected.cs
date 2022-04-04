using System;
using System.Net.Sockets;
using ChatServer.Model;

namespace ChatServer
{
    public class ClientConnected
    {
        public User Identity { get; set; }
        public Socket Connection { get; set; }
        public Guid ClientId { get; set; }
    }
}
