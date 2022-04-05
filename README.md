# ChatServer

    cd ChatServer
    dotnet run

For testing 

    dotnet test

The ChatServer run on localhost and listen TCP connection on port 11000

The ChatServer expose a list of Rest API for interaction with chatroom

## REST Api

### List of client connected

    GET https://localhost:5001/api/client

### List of all shared messages

    GET https://localhost:5001/api/message

### Retrive the number of message sent by a specific user id

    GET https://localhost:5001/api/message/count-by-user?id={userid}

### Post a message in the chat room

    POST https://localhost:5001/api/message
    {
        "NickName":"Lenny",
        "Text":"Hi all"
    }      

Enjoy