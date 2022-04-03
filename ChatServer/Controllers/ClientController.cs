using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ChatServer.Controllers
{
    [Route("api/[controller]")]
    public class ClientController : Controller
    {
        ChatRoom room;

        public ClientController(ChatRoom chatRoom)
        {
            room = chatRoom;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(room.GetClientConnected());
        }

    }
}
