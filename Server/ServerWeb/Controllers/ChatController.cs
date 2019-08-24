using ChatCore.Engine;
using ChatServer.Engine.Network;
using Microsoft.AspNetCore.Mvc;
using ServerWeb.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServerWeb.Controllers
{
    public class ChatController : Controller
    {
        private readonly ServerHandler ServerHandler;

        public ChatController(ServerHandler serverHandler)
        {
            ServerHandler = serverHandler;
        }
        public IActionResult Index()
        {
            var user = new User()
            {
                Username = "Sakib"
            };
            return View(new ChatViewModel(user,new List<User>()));
        }

        [HttpGet]
        public async Task GetSocket()
        {
            var context = ControllerContext.HttpContext;
            var isSocketRequest = context.WebSockets.IsWebSocketRequest;

            if (isSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                await ServerHandler.Start(webSocket);
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }
    }
}