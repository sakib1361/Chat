using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ChatCore.Model.Core;
using Microsoft.AspNetCore.Authorization;

namespace ServerWeb.Engine.Database
{
    [Authorize(Roles =ChatConstants.AdminRole)]
    public class AdminController : Controller
    {
        private readonly UserManager<IDUser> _manager;

        public AdminController(UserManager<IDUser> manager)
        {
            _manager = manager;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            return View(await _manager.GetUsersInRoleAsync(ChatConstants.MemberRole));
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _manager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _manager.FindByIdAsync(id);
            await _manager.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }
    }
}
