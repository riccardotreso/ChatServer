using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ChatServer.Controllers
{
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        ChatRoom room;
        public MessageController(ChatRoom chatRoom)
        {
            room = chatRoom;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(room.GetAllMessages());
        }

        [HttpGet("count-by-user")]
        public IActionResult GetCountMessageFromUserId([FromQuery] string id)
        {
            return Ok(room.GetCountMessageFromUserId(id));
        }

        [HttpPost]
        public IActionResult Post([FromBody] Message message)
        {
            room.AddMessageAndPropagate(message);
            return Accepted(message);
        }

    }
}
