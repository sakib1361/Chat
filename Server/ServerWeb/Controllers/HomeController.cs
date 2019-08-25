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

        public HomeController(ServerHandler serverHandler, UserManager<IDUser> userManager)
        {
            ServerHandler = serverHandler;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return View(new ChatViewModel(user, new List<IDUser>()));
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
