using System.Threading.Tasks;
using System.Web;
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
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _apiHandler.GetUsers());
        }

        [Authorize]
        public async Task<IActionResult> GetHistory(string username, string receivername)
        {
            return Ok(await _apiHandler.GetUserHistory(username, receivername));
        }

        public async Task<IActionResult> Login(string username, string password)
        {
            var res = await _signinManager
                .PasswordSignInAsync(username.Trim(), password.Trim(), true, true);
            if (res == Microsoft.AspNetCore.Identity.SignInResult.Success)
            {
                var user = await _usermanager.FindByNameAsync(username);
                var token = await _usermanager.GenerateUserTokenAsync(user, "Default", "Chat");
                return Ok(HttpUtility.UrlEncode(token));
            }
            else return BadRequest();
        }

        public async Task<IActionResult> Register(string firstname, string lastname, string username, string password)
        {
            var user = new IDUser()
            {
                FirstName = firstname,
                LastName = lastname,
                UserName = username,
            };
            var res = await _usermanager.CreateAsync(user, password);
            if (res == IdentityResult.Success)
            {
                var dbUser = await _usermanager.FindByNameAsync(username);
                var token = await _usermanager.GenerateUserTokenAsync(dbUser, "Default", "Chat");
                return Ok(HttpUtility.UrlEncode(token));
            }
            else return BadRequest(res.Errors);
        }
    }
}