using System.Threading.Tasks;
using ChatServer.Engine.Database;
using ChatServer.Engine.Network;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ServerWeb.Controllers
{
    public class APIController : Controller
    {
        private readonly APIHandler _apiHandler;
        private readonly UserManager<IDUser> _usermanager;
        private readonly SignInManager<IDUser> _signinManager;

        public APIController(APIHandler aPIHandler,
            UserManager<IDUser> userManager,
            SignInManager<IDUser> signInManager)
        {
            _apiHandler = aPIHandler;
            _usermanager = userManager;
            _signinManager = signInManager;
        }
        public IActionResult Index()
        {
            return Ok("The Api is working");
        }

        [Authorize]
        public IActionResult GetUsers()
        {
            return Ok(_apiHandler.GetUsers());
        }

        public async Task<IActionResult> Login(string username, string password)
        {
            var res = await _signinManager
                .PasswordSignInAsync(username.Trim(), password.Trim(), true, true);
            if (res == Microsoft.AspNetCore.Identity.SignInResult.Success)
            {
                var user = await _usermanager.FindByNameAsync(username);
                var token = await _usermanager.GenerateUserTokenAsync(user, "Server", "Chat");
                return Ok(token);
            }
            else return BadRequest();
        }
    }
}