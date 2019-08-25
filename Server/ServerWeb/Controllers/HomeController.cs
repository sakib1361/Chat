using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatCore.Engine;
using ChatServer.Engine.Database;
using ChatServer.Engine.Network;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServerWeb.Models;

namespace ServerWeb.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ServerHandler ServerHandler;
        private readonly UserManager<IDUser> _userManager;
        private readonly APIHandler _apiHandler;

        public HomeController(ServerHandler serverHandler, UserManager<IDUser> userManager, APIHandler aPIHandler)
        {
            ServerHandler = serverHandler;
            _userManager = userManager;
            _apiHandler = aPIHandler;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var allUser = await _apiHandler.GetUsers();
            return View(new ChatViewModel(user, allUser));
        }

        public async Task<IActionResult> GetHistory(string receivername)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var res = await _apiHandler.GetUserHistory(user.UserName, receivername);

            res = new List<ChatObject>()
            {
                new ChatObject(MessageType.EndToEnd)
                {
                    Id = 1,
                    SenderName = user.UserName,
                    ReceiverName = receivername,
                    Message = "How are you"
                },
                new ChatObject(MessageType.EndToEnd)
                {
                    Id = 2,
                    SenderName = receivername,
                    ReceiverName = user.UserName,
                    Message = "Fine and you?"
                },
            };
            var data= PartialView("_ChatView",new ChatViewModel(user, res));
            return data;
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
