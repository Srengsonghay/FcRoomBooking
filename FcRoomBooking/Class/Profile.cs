using FcRoomBooking.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FcRoomBooking.Class
{
    public class Profile : Controller
    {
        public readonly ApplicationDbContext _applicationDbContext;
        public readonly UserManager<ApplicationUser> userManager;
        public readonly RoleManager<ApplicationRole> roleManager;
        public Profile(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager)
        {
            _applicationDbContext = applicationDbContext;
            this.userManager = userManager;
            
        }
        public Profile(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _applicationDbContext = applicationDbContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public async Task<IActionResult> UserProfile(string id)
        {
            var profile = await _applicationDbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
            ViewData["UserProfile"] = profile;
            return View(profile);
        }
    }
}
