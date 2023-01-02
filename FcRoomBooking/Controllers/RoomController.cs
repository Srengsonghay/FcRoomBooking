using FcRoomBooking.Areas.Identity.Data;
using FcRoomBooking.Constants;
using FcRoomBooking.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FcRoomBooking.Controllers
{
    public class RoomController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public RoomController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IActionResult List()
        {
            var Room = _applicationDbContext.Rooms.ToList();
            return View(Room);
        }
        [Authorize(Permissions.Room.Create)]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost, Authorize(Permissions.Room.Create)]
        public async Task<IActionResult> Create(Room room, List<IFormFile> image)
        {
            if (ModelState.IsValid)
            {
                foreach (var item in image)
                {
                    if (item.Length > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            await item.CopyToAsync(stream);
                            room.RoomImage = stream.ToArray();
                        }
                    }
                }

                await _applicationDbContext.Rooms.AddAsync(room);
                await _applicationDbContext.SaveChangesAsync();

                return RedirectToAction("Create");
            }
            return View(room);
        }
        [HttpGet, Authorize(Permissions.Room.Edit)]
        public async Task<IActionResult> Edit(int id)
        {
            var room = await _applicationDbContext.Rooms.FirstOrDefaultAsync(x => x.Id == id);
            return View(room);
        }
        [HttpPost, Authorize(Permissions.Room.Edit)]
        public async Task<IActionResult> Edit(Room model)
        {
            var room = await _applicationDbContext.Rooms.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (model.image != null)
            {
                if (model.image.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        await model.image.CopyToAsync(stream);
                        model.RoomImage = stream.ToArray();
                    }
                }
            }
            else
            {
                model.RoomImage = room?.RoomImage;
            }
            room.RoomImage = model.RoomImage;
            room.RoomName = model.RoomName;
            room.RoomDescription = model.RoomDescription;
            _applicationDbContext.Rooms.Update(room);
            await _applicationDbContext.SaveChangesAsync();
            return RedirectToAction("List");
        }
        [HttpPost, Authorize(Permissions.Room.Delete)]
        public async Task<IActionResult> Delete(Room model)
        {
            var room = await _applicationDbContext.Rooms.FindAsync(model.Id);
            _applicationDbContext.Rooms.Remove(room);
            await _applicationDbContext.SaveChangesAsync();
            return RedirectToAction("List");
        }
    }
}

