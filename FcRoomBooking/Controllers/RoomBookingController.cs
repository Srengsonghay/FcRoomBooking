using FcRoomBooking.Areas.Identity.Data;
using FcRoomBooking.Models.Domain;
using FcRoomBooking.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FcRoomBooking.Controllers
{
    public class RoomBookingController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;

        public RoomBookingController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public IActionResult Index()
        {
            CheckBookingViewModel model = new CheckBookingViewModel();
            model.RoomBooking = dbContext.RoomBookings.OrderByDescending(x=>x.BookingTo).Include(x => x.Room).Include(x => x.ApplicationUser).Where(x => x.BookingStatus == "Active" || x.BookingStatus == "Ongoing").ToList();
            model.Participant = dbContext.Participants.ToList();
            return View(model);
        }
        [HttpGet("RoomBooking/{id}")]
        public IActionResult Index(int id)
        {
            CheckBookingViewModel model = new CheckBookingViewModel();
            model.RoomBooking = dbContext.RoomBookings.OrderByDescending(x => x.BookingTo).Include(x => x.Room).Include(x => x.ApplicationUser).Where(x => x.BookingStatus == "Active" || x.BookingStatus == "Ongoing").Where(x => x.Id == id).ToList();
            model.Participant = dbContext.Participants.ToList();
            return View(model);
        }
        public IActionResult Participant(int id)
        {
            var list = dbContext.Participants.Include(x => x.ApplicationUser).ToList().FindAll(x => x.RoomBookingId == id);
            var newlist = new List<ParticipantViewModel>();
            if (list.Any())
            {
                foreach (var item in list)
                {
                    newlist.Add(new ParticipantViewModel
                    {
                        RoomBookingId = id,
                        Username = item.ApplicationUser.Name,
                        Email = item.ApplicationUser.Email,
                        Reason = item.Reason,
                        IsExcept = item.IsExcept,
                        Id = item.Id,

                    });
                }
                ViewBag.BookingId = id;
            }
            return View(newlist);

        }
        [HttpGet]
        public IActionResult show(int Id)
        {
            var show = dbContext.Participants.FirstOrDefault(x => x.Id == Id);
            return PartialView("_ReasonPartialView", show);

        }
        [HttpGet]
        public IActionResult Responce(int id)
        {
            var user = userManager.GetUserId(User);
            ResponceViewModel view = new ResponceViewModel();
            view.RoomBooking = dbContext.RoomBookings.Include(x => x.ApplicationUser).Include(x => x.Room).FirstOrDefault(x => x.Id == id);
            view.Participant = dbContext.Participants.Where(x => x.RoomBookingId == id).Where(x => x.UserId == user.ToString()).FirstOrDefault();

            return View(view);
        }
        
        [HttpPost]
        public IActionResult Accept(ResponceViewModel model)
        {
            var participant = dbContext.Participants.Where(x=>x.UserId == model.Participant.UserId).FirstOrDefault(x=>x.RoomBookingId == model.RoomBooking.Id);
            if(model.Participant.IsExcept == true)
            {
                participant.IsExcept = model.Participant.IsExcept;
                participant.Reason = null;
                TempData["isJoin"] = true;
                

            }
            else if(model.Participant.IsExcept == false)
            {
                participant.IsExcept = model.Participant.IsExcept;
                participant.Reason = model.Participant.Reason;
                TempData["notAvailable"] = true;
            }
            dbContext.Participants.Update(participant);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
           
        }
    }
}
