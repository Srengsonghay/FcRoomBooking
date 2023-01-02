using FcRoomBooking.Areas.Identity.Data;
using FcRoomBooking.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FcRoomBooking.Controllers
{
    //[Authorize]
    [Authorize(Permissions.User.View)]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var allUsersExceptCurrentUser = await _userManager.Users.Where(a => a.Id != currentUser.Id).ToListAsync();
            return View(allUsersExceptCurrentUser);
        }
        [HttpGet, Authorize(Permissions.User.Edit)]
        public async Task<IActionResult> Edit(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return View(user);
        }
        [HttpPost, Authorize(Permissions.User.Edit)]
        public async Task<IActionResult> Edit(ApplicationUser model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                user.Name = model.Name;
                user.Email = model.Email;
                user.UserName = model.Email;
                await _userManager.UpdateAsync(user);
                TempData["isUpdated"] = true;
                return RedirectToAction("Index");
            }
            return NotFound();
        }
        [HttpPost, Authorize(Permissions.User.Delete)]
        public async Task<IActionResult> Delete(ApplicationUser model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            await _userManager.DeleteAsync(user);
            TempData["notAvailable"] = true;
            return RedirectToAction("Index");
            
        }
    }
}
