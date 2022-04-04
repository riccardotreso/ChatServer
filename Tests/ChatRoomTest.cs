using System;
using Xunit;
using ChatServer;
using ChatServer.Model;

namespace Tests
{
    public class ChatRoomTest
    {
        ChatRoom room;
        public ChatRoomTest()
        {
            room = new ChatRoom();
        }

        [Theory]
        [InlineData("Riccardo")]
        public void ConnectFirst(string nickname)
        {
            ChatCommand chatCommand = ChatCommandFactory.Login(nickname);
            _ = room.GetChatResponseFromCommand(chatCommand, null, Guid.NewGuid());

            var result = room.GetClientConnected().FindIndex(x => x.Value.Equals(nickname));
            Assert.True(result > -1);
        }

        [Theory]
        [InlineData("Mario")]
        public void ConnectSameNickName(string nickname)
        {
            ChatCommand chatCommand = ChatCommandFactory.Login(nickname);
            var firstResponse = room.GetChatResponseFromCommand(chatCommand, null, Guid.NewGuid());
            var secondResponse = room.GetChatResponseFromCommand(chatCommand, null, Guid.NewGuid());

            Assert.True(!firstResponse.IsError);
            Assert.True(secondResponse.IsError);

        }

        [Theory]
        [InlineData("Mario", "Prova123")]
        [InlineData("Riccardo", "Prova345")]
        public void PostData(string nickname, string text)
        {
            ChatCommand chatCommand = ChatCommandFactory.Text(new User(nickname), text);
            _ = room.GetChatResponseFromCommand(chatCommand, null, Guid.NewGuid());

            Assert.Contains($"{nickname} says >> {text}", room.GetAllMessages());

        }

        [Theory]
        [InlineData("Luca", "io sono luca")]
        public void GetMessageByUserId(string nickname, string text)
        {
            ChatCommand chatCommand = ChatCommandFactory.Login(nickname);
            var response = room.GetChatResponseFromCommand(chatCommand, null, Guid.NewGuid());


            chatCommand = ChatCommandFactory.Text(new User(nickname), text);
            _ = room.GetChatResponseFromCommand(chatCommand, null, Guid.NewGuid());

            Assert.True(1 == room.GetCountMessageFromUserId(response.Data));

        }

        [Theory]
        [InlineData("Homer", "another beer please")]
        public void PostMessageFromApi(string nickname, string text)
        {
            Message message = new Message() { NickName = nickname, Text = text };
            room.AddMessageAndPropagate(message);

            Assert.Contains($"{nickname} says >> {text}", room.GetAllMessages());
        }

    }
}
