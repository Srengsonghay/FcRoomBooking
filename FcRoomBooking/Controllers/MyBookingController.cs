using FcRoomBooking.Areas.Identity.Data;
using FcRoomBooking.Class;
using FcRoomBooking.Constants;
using FcRoomBooking.Models.Domain;
using FcRoomBooking.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FcRoomBooking.Controllers
{
    [Authorize]
    public class MyBookingController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;

        public MyBookingController(ApplicationDbContext dbContext,UserManager<ApplicationUser> userManager,RoleManager<ApplicationRole> roleManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var user = userManager.GetUserId(User);
            var list = dbContext.RoomBookings.Include(x=>x.Room).Include(x=>x.ApplicationUser).Where(x=>x.UserId == user).ToList();
            return View(list);
        }
        [HttpGet("MyBooking/{filter}")]
        public IActionResult Index(string filter)
        {
            var user = userManager.GetUserId(User);
            var list = dbContext.RoomBookings.Include(x=>x.Room).Include(x=>x.ApplicationUser).Where(x=>x.UserId == user && x.BookingStatus == filter).ToList();
            return View(list);
        }
        [Authorize(Permissions.Booking.Create)]
        [HttpGet("MyBooking/Create")]
        public IActionResult Create(string? date)
        {
            ViewBag.TimeList = TimePicker();
            ViewBag.roomList = RoomList();
            ViewBag.partList = ParticipantList();
            ViewBag.BookingId = TempData["RoomBookingId"];
            if(ViewBag.BookingId != null)
            {
                int id = (int)TempData["RoomBookingId"];
                var room = dbContext.RoomBookings.FirstOrDefault(x => x.Id == id);
                var newRoom = new RoomBookingViewModel()
                {
                    Id = room.Id,
                    RoomId = room.RoomId,
                    RoomName = room.Room.RoomName,
                    Subject = room.Subject,
                    Detail = room.Detail,
                    UserId = room.UserId,
                    BookingFrom = room.BookingFrom.ToString("yyyy-MM-dd"),
                    BookingFromTime = room.BookingFrom.ToShortTimeString().Substring(0, 5).Trim(),
                    BookingTo = room.BookingTo.ToString("yyyy-MM-dd"),
                    BookingToTime = room.BookingTo.ToShortTimeString().Substring(0, 5).Trim()
                };
                return View(newRoom);
            }
            else
            {
                if (date != null)
                {
                    var newRoom = new RoomBookingViewModel()
                    {
                        BookingFrom = date,
                        BookingTo = date,
                    };
                    return View(newRoom);
                }
            }
            return View();
        }
        public IActionResult Participant(int id)
        {
            ViewBag.partList = ParticipantList();
            var list = dbContext.Participants.ToList().FindAll(x=>x.RoomBookingId== id);
            if(list.Any())
            {
                var newlist = new List<ParticipantViewModel>();
                foreach (var item in list)
                {
                    newlist.Add(new ParticipantViewModel
                    {
                        RoomBookingId = id,
                        Username = item.ApplicationUser.UserName,
                        Email = item.ApplicationUser.Email,
                        userId = item.UserId
                        
                    });
                }
                ViewBag.BookingId = id;
                
                return View(newlist);
            }
            else
            {
                ViewBag.BookingId = id;
                return View();

            }

        }
        [HttpPost]
        public async Task<IActionResult> PostParticipant(ParticipantViewModel Request,int id)
        {
            foreach (var item in Request.ParticipantId)
            {
                var user = await userManager.FindByIdAsync(item);
                var check = dbContext.Participants.Where(x=>x.UserId == item && x.RoomBookingId == id);
                if (check.Any())
                {
                    TempData["Dupe"] = check.First().ApplicationUser.UserName;
                    return RedirectToAction("Participant",new {id=id});
                }
                dbContext.Participants.Add(new Participant
                {
                    UserId= user.Id,
                    RoomBookingId= id,
                });

            }
            dbContext.SaveChanges();
            TempData["isInvited"] = true;
            return RedirectToAction("Participant",new {id=id});
        }
        [Authorize(Permissions.Booking.Edit)]
        public IActionResult Edit(int id)
        {
            var room = dbContext.RoomBookings.FirstOrDefault(x=>x.Id == id);
            ViewBag.TimeList = TimePicker();
            ViewBag.roomList = RoomList();
            var newRoom = new RoomBookingViewModel()
            {
                Id = room.Id,
                RoomId = room.RoomId,
                RoomName = room.Room.RoomName,
                Subject = room.Subject,
                Detail = room.Detail,
                UserId= room.UserId,
                BookingFrom = room.BookingFrom.ToString("yyyy-MM-dd"),
                BookingFromTime = room.BookingFrom.ToShortTimeString().Substring(0,5).Trim(),
                BookingTo = room.BookingTo.ToString("yyyy-MM-dd"),
                BookingToTime = room.BookingTo.ToShortTimeString().Substring(0,5).Trim()
            };
            return View(newRoom);
        }
        [HttpPost, Authorize(Permissions.Booking.Edit)]
        public async Task<IActionResult> Post(RoomBookingViewModel Request)
        {
            DateTime dateFrom = DateTime.Parse(Request.BookingFrom +" "+ Request.BookingFromTime);
            DateTime dateTo = DateTime.Parse(Request.BookingTo + " " + Request.BookingToTime);
            var user = await userManager.GetUserAsync(User);
            
            var check = await dbContext.RoomBookings.FirstOrDefaultAsync(x => x.BookingFrom <= dateTo && x.BookingTo >= dateFrom && x.RoomId == Request.RoomId && x.BookingStatus == "Active");
            if(check == null)
            {
                await dbContext.RoomBookings.AddAsync(new RoomBooking()
                {
                    Id = Request.Id,
                    RoomId = Request.RoomId,
                    UserId = user.Id,
                    Subject = Request.Subject,
                    Detail = Request.Detail,
                    BookingFrom = dateFrom,
                    BookingTo = dateTo,
                    BookingStatus = "Active"
                });
                dbContext.SaveChanges();
                var room = dbContext.RoomBookings.FirstOrDefaultAsync(x => x.RoomId == Request.RoomId && x.UserId == user.Id && x.BookingFrom == dateFrom && x.BookingTo == dateTo && x.Subject == Request.Subject).Result;

                foreach (var item in Request.participantViewModel.ParticipantId)
                {
                    dbContext.Participants.Add(new Participant
                    {
                        UserId = item,
                        RoomBookingId = room.Id,
                    });

                }
                dbContext.SaveChanges();
                TempData["isCreated"] = true;
                return RedirectToAction("Index", new {filter="active"});
            }
            else
            {
                TempData["notAvailable"] = true;
                return RedirectToAction("Create");
            }
        }
        [HttpGet]
        public IActionResult Remove(string id,int url)
        {
            var user = dbContext.Participants.First(x=>x.UserId == id);
            dbContext.Participants.Remove(user);
            dbContext.SaveChanges();
            TempData["isRemoved"] = true;
            return RedirectToAction("Participant",new {id=url});
        }
        [HttpPost, Authorize(Permissions.Booking.Delete)]
        public IActionResult Update(RoomBookingViewModel Request,int id,string? isCancel)
        {
            var booking = dbContext.RoomBookings.FirstOrDefault(x=>x.Id == id);
            if(isCancel == "true")
            {
                booking.BookingStatus = "Cancelled";
                dbContext.SaveChanges();
                TempData["isCancel"] = true; 
                return RedirectToAction("Index");
            }
            else
            {
                if (booking != null)
                {
                    DateTime dateFrom = DateTime.Parse(Request.BookingFrom + " " + Request.BookingFromTime);
                    DateTime dateTo = DateTime.Parse(Request.BookingTo + " " + Request.BookingToTime);
                    var check =  dbContext.RoomBookings.FirstOrDefault(x => x.BookingFrom <= dateTo && x.BookingTo >= dateFrom && x.RoomId == Request.RoomId && x.BookingStatus == "Active");
                    if(check == null)
                    {
                        booking.RoomId = Request.RoomId;
                        booking.Subject = Request.Subject;
                        booking.Detail = Request.Detail;
                        booking.BookingFrom = dateFrom;
                        booking.BookingTo = dateTo;
                        dbContext.SaveChanges();
                        TempData["isUpdated"] = true;
                        return RedirectToAction("index");
                    }
                    else
                    {
                        TempData["notAvailable"] = true;
                        return RedirectToAction("Edit", new {id=Request.Id});
                    }
                }
                return NotFound();
            }
        }
        
        public IActionResult Delete(int id)
        {
            return View();
        }

        public List<SelectListItem> TimePicker()
        {
            List<SelectListItem> List = new List<SelectListItem>();
            for (int i = 0; i < 24; i++)
            {
                for(float y = 0; y < 31; y += 30)
                {
                    List.Add(new SelectListItem
                    {
                        Text = $"{i}:{(y!=0?y:"00")}",
                        Value = $"{i}:{(y != 0 ? y : "00")}"
                    });
                }
            }
            return List;
        }
        public List<SelectListItem> RoomList()
        {
            List<SelectListItem> List = new List<SelectListItem>();
            var roomList = dbContext.Rooms.ToList();
            foreach (var item in roomList)
            {
                List.Add(new SelectListItem
                {
                    Text = item.RoomName,
                    Value = item.Id.ToString()
                });
            }
            return List;
        }
        public List<SelectListItem> ParticipantList()
        {
            List<SelectListItem> List = new List<SelectListItem>();
            var list = userManager.Users.ToList().Where(x => x.UserName != User.Identity.Name);
            foreach (var item in list)
            {
                List.Add(new SelectListItem
                {
                    Text=item.UserName,Value = item.Id.ToString()
                });
            }
            return List;
        }
    }
}
