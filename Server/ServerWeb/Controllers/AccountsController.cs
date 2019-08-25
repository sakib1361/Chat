using ChatServer.Engine.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ServerWeb.Models;
using System.Threading.Tasks;

namespace ServerWeb.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly UserManager<IDUser> _userManager;
        private readonly SignInManager<IDUser> _signInManager;

        public AccountsController(
            UserManager<IDUser> userManager,
            SignInManager<IDUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager; 
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var username = model.UserName.Trim();
                var password = model.Password.Trim();
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(username, password, true, true);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", nameof(ChatController));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await Register(model.FirstName, model.LastName, model.UserName, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", nameof(ChatController));
                }
                AddError(result,ModelState);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private async Task<IdentityResult> Register(
            string firstname,
            string lastname,
            string username,
            string password)
        {
            var user = new IDUser
            {
                FirstName = firstname,
                LastName = lastname,
                UserName = username
            };
            return await _userManager.CreateAsync(user, password);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        private static void AddError(IdentityResult result, ModelStateDictionary modelState)
        {
            foreach (var error in result.Errors)
            {
                modelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}