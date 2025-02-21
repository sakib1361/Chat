﻿using System.Threading.Tasks;
using System.Web;
using ChatCore.Engine;
using ChatCore.Model.Core;
using ChatServer.Engine.Network;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServerWeb.Engine.Database;
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
            if (user == null)
                return RedirectToAction("Login", "Accounts");

            var role = await _userManager.GetRolesAsync(user);
            if (role.Contains(ChatConstants.AdminRole))
                return RedirectToAction("Index", "Admin");
            else
            {
                var allUser = await _apiHandler.GetUsers();
                var token = await _userManager.GenerateUserTokenAsync(user, "Default", "Chat");
                var shadowUser = new User(user.UserName, user.FirstName, user.LastName);
                return View(new ChatViewModel(shadowUser, allUser, HttpUtility.UrlEncode(token)));
            }
        }

        public async Task<IActionResult> GetHistory(string receivername)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var res = await _apiHandler.GetUserHistory(user.UserName, receivername);
            var receiver = await _userManager.FindByNameAsync(receivername);
            var shadowUser = new User(user.UserName, user.FirstName, user.LastName);
            var shadowReciever = new User(receiver.UserName, receiver.FirstName, receiver.LastName);
            var data= PartialView("_ChatView",new ChatViewModel(shadowUser, shadowReciever, res));
            return data;
        }

        public async Task<IActionResult> GetChatView(ChatObject chatObject)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (chatObject.SenderName == user.UserName)
            {
                return PartialView("_senderChat", chatObject);
            }
            else
            {
                return PartialView("_receiverChat", chatObject);
            }
        }

        [HttpGet]
        [AllowAnonymous]
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
