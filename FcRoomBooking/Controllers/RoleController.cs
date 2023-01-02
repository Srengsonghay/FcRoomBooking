using FcRoomBooking.Areas.Identity.Data;
using FcRoomBooking.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FcRoomBooking.Controllers
{
    [Authorize(Roles="Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        public RoleController(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var role = new RoleViewModel();
            role.Roles = await _roleManager.Roles.ToListAsync();
            ViewBag.type = TempData["type"];
            ViewBag.Message = TempData["Message"];
            return View(role);
        }
        [HttpPost]
        public async Task<IActionResult> AddRole(string roleName)
        {
            if (roleName != null)
            {
                await _roleManager.CreateAsync(new ApplicationRole(roleName.Trim()));
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string roleid)
        {

            var rolename = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == roleid);
            RoleViewModel role = new RoleViewModel();
            role.Id = rolename.Id;
            role.Name = rolename.Name;
            return View(role);
        }
        [HttpPost]
        public async Task<IActionResult> EditRole(ApplicationRole model)
        {
            ApplicationRole role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == model.Id);
            role.Name = model.Name;
            await _roleManager.UpdateAsync(role); 
            TempData["type"] = "info";
            TempData["Message"] = "The role you selected has been updated successfully!";

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var roleRemove = await _roleManager.Roles.Where(x=>x.Islocked !=true).FirstOrDefaultAsync(x => x.Id == id);
            RoleViewModel role = new RoleViewModel();
            if(roleRemove != null)
            {
                await _roleManager.DeleteAsync(roleRemove);
                TempData["type"] = "success";
                TempData["Message"] = "The role you selected has been deleted successfully!";
            }
            else
            {
                TempData["type"] = "danger";
                TempData["Message"] = "The role you selected can not be deleted!";
            }
            
            return RedirectToAction("Index");
        }


    }
}