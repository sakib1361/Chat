using ChatCore.Model.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServerWeb.Engine.Database;
using System.Threading.Tasks;

namespace ServerWeb.Controllers
{
    [Authorize(Roles = ChatConstants.AdminRole)]
    public class AdminController : Controller
    {
        private readonly UserManager<IDUser> _usermanager;

        public AdminController(UserManager<IDUser> userManager)
        {
            _usermanager = userManager;
        }

        public IActionResult Index()
        {
            return View(_usermanager.GetUsersInRoleAsync(ChatConstants.MemberRole));
        }

        public async Task<IActionResult> Delete(string userId)
        {
            var user = await _usermanager.FindByIdAsync(userId);
            if (user == null)
            {
                
            }
            else
            {
                await _usermanager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }
    }
}