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
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

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
        public IActionResult SendEmail(ParticipantViewModel request,int id)
        {
            var list = dbContext.Participants.Include(x=>x.ApplicationUser).ToList().FindAll(x=>x.RoomBookingId== id);
            var user = userManager.GetUserAsync(HttpContext.User).Result;
            var userEmail = user.Email;
            if (list.Any())
            {
                foreach (var item in list)
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(user.UserName,userEmail ));
                    message.To.Add(MailboxAddress.Parse(item.ApplicationUser.Email));
                    message.Subject = request.Subject;
                    message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                    {
                        Text = "<!doctype html>\r\n<html>\r\n  <head>\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"/>\r\n    <meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" />\r\n    <title>Simple Transactional Email</title>\r\n    <style>\r\n      /* -------------------------------------\r\n          GLOBAL RESETS\r\n      ------------------------------------- */\r\n      \r\n      /*All the styling goes here*/\r\n      \r\n      img {\r\n        border: none;\r\n        -ms-interpolation-mode: bicubic;\r\n        max-width: 100%; \r\n      }\r\n\r\n      body {\r\n        background-color: #f6f6f6;\r\n        font-family: sans-serif;\r\n        -webkit-font-smoothing: antialiased;\r\n        font-size: 14px;\r\n        line-height: 1.4;\r\n        margin: 0;\r\n        padding: 0;\r\n        -ms-text-size-adjust: 100%;\r\n        -webkit-text-size-adjust: 100%; \r\n      }\r\n\r\n      table {\r\n        border-collapse: separate;\r\n        mso-table-lspace: 0pt;\r\n        mso-table-rspace: 0pt;\r\n        width: 100%; }\r\n        table td {\r\n          font-family: sans-serif;\r\n          font-size: 14px;\r\n          vertical-align: top; \r\n      }\r\n\r\n      /* -------------------------------------\r\n          BODY & CONTAINER\r\n      ------------------------------------- */\r\n\r\n      .body {\r\n        background-color: #f6f6f6;\r\n        width: 100%; \r\n      }\r\n\r\n      /* Set a max-width, and make it display as block so it will automatically stretch to that width, but will also shrink down on a phone or something */\r\n      .container {\r\n        display: block;\r\n        margin: 0 auto !important;\r\n        /* makes it centered */\r\n        max-width: 580px;\r\n        padding: 10px;\r\n        width: 580px; \r\n      }\r\n\r\n      /* This should also be a block element, so that it will fill 100% of the .container */\r\n      .content {\r\n        box-sizing: border-box;\r\n        display: block;\r\n        margin: 0 auto;\r\n        max-width: 580px;\r\n        padding: 10px; \r\n      }\r\n\r\n      /* -------------------------------------\r\n          HEADER, FOOTER, MAIN\r\n      ------------------------------------- */\r\n      .main {\r\n        background: #ffffff;\r\n        border-radius: 3px;\r\n        width: 100%; \r\n      }\r\n\r\n      .wrapper {\r\n        box-sizing: border-box;\r\n        padding: 20px; \r\n      }\r\n\r\n      .content-block {\r\n        padding-bottom: 10px;\r\n        padding-top: 10px;\r\n      }\r\n\r\n      .footer {\r\n        clear: both;\r\n        margin-top: 10px;\r\n        text-align: center;\r\n        width: 100%; \r\n      }\r\n        .footer td,\r\n        .footer p,\r\n        .footer span,\r\n        .footer a {\r\n          color: #999999;\r\n          font-size: 12px;\r\n          text-align: center; \r\n      }\r\n\r\n      /* -------------------------------------\r\n          TYPOGRAPHY\r\n      ------------------------------------- */\r\n      h1,\r\n      h2,\r\n      h3,\r\n      h4 {\r\n        color: #000000;\r\n        font-family: sans-serif;\r\n        font-weight: 400;\r\n        line-height: 1.4;\r\n        margin: 0;\r\n        margin-bottom: 30px; \r\n      }\r\n\r\n      h1 {\r\n        font-size: 35px;\r\n        font-weight: 300;\r\n        text-align: center;\r\n        text-transform: capitalize; \r\n      }\r\n\r\n      p,\r\n      ul,\r\n      ol {\r\n        font-family: sans-serif;\r\n        font-size: 14px;\r\n        font-weight: normal;\r\n        margin: 0;\r\n        margin-bottom: 15px; \r\n      }\r\n        p li,\r\n        ul li,\r\n        ol li {\r\n          list-style-position: inside;\r\n          margin-left: 5px; \r\n      }\r\n\r\n      a {\r\n        color: #3498db;\r\n        text-decoration: underline; \r\n      }\r\n\r\n      /* -------------------------------------\r\n          BUTTONS\r\n      ------------------------------------- */\r\n      .btn {\r\n        box-sizing: border-box;\r\n        width: 100%; }\r\n        .btn > tbody > tr > td {\r\n          padding-bottom: 15px; }\r\n        .btn table {\r\n          width: auto; \r\n      }\r\n        .btn table td {\r\n          background-color: #ffffff;\r\n          border-radius: 5px;\r\n          text-align: center; \r\n      }\r\n        .btn a {\r\n          background-color: #ffffff;\r\n          border: solid 1px #3498db;\r\n          border-radius: 5px;\r\n          box-sizing: border-box;\r\n          color: #3498db;\r\n          cursor: pointer;\r\n          display: inline-block;\r\n          font-size: 14px;\r\n          font-weight: bold;\r\n          margin: 0;\r\n          padding: 12px 25px;\r\n          text-decoration: none;\r\n          text-transform: capitalize; \r\n      }\r\n\r\n      .btn-primary table td {\r\n        background-color: #3498db; \r\n      }\r\n\r\n      .btn-primary a {\r\n        background-color: #3498db;\r\n        border-color: #3498db;\r\n        color: #ffffff; \r\n      }\r\n\r\n      /* -------------------------------------\r\n          OTHER STYLES THAT MIGHT BE USEFUL\r\n      ------------------------------------- */\r\n      .last {\r\n        margin-bottom: 0; \r\n      }\r\n\r\n      .first {\r\n        margin-top: 0; \r\n      }\r\n\r\n      .align-center {\r\n        text-align: center; \r\n      }\r\n\r\n      .align-right {\r\n        text-align: right; \r\n      }\r\n\r\n      .align-left {\r\n        text-align: left; \r\n      }\r\n\r\n      .clear {\r\n        clear: both; \r\n      }\r\n\r\n      .mt0 {\r\n        margin-top: 0; \r\n      }\r\n\r\n      .mb0 {\r\n        margin-bottom: 0; \r\n      }\r\n\r\n      .preheader {\r\n        color: transparent;\r\n        display: none;\r\n        height: 0;\r\n        max-height: 0;\r\n        max-width: 0;\r\n        opacity: 0;\r\n        overflow: hidden;\r\n        mso-hide: all;\r\n        visibility: hidden;\r\n        width: 0; \r\n      }\r\n\r\n      .powered-by a {\r\n        text-decoration: none; \r\n      }\r\n\r\n      hr {\r\n        border: 0;\r\n        border-bottom: 1px solid #f6f6f6;\r\n        margin: 20px 0; \r\n      }\r\n\r\n      /* -------------------------------------\r\n          RESPONSIVE AND MOBILE FRIENDLY STYLES\r\n      ------------------------------------- */\r\n      @media only screen and (max-width: 620px) {\r\n        table.body h1 {\r\n          font-size: 28px !important;\r\n          margin-bottom: 10px !important; \r\n        }\r\n        table.body p,\r\n        table.body ul,\r\n        table.body ol,\r\n        table.body td,\r\n        table.body span,\r\n        table.body a {\r\n          font-size: 16px !important; \r\n        }\r\n        table.body .wrapper,\r\n        table.body .article {\r\n          padding: 10px !important; \r\n        }\r\n        table.body .content {\r\n          padding: 0 !important; \r\n        }\r\n        table.body .container {\r\n          padding: 0 !important;\r\n          width: 100% !important; \r\n        }\r\n        table.body .main {\r\n          border-left-width: 0 !important;\r\n          border-radius: 0 !important;\r\n          border-right-width: 0 !important; \r\n        }\r\n        table.body .btn table {\r\n          width: 100% !important; \r\n        }\r\n        table.body .btn a {\r\n          width: 100% !important; \r\n        }\r\n        table.body .img-responsive {\r\n          height: auto !important;\r\n          max-width: 100% !important;\r\n          width: auto !important; \r\n        }\r\n      }\r\n\r\n      /* -------------------------------------\r\n          PRESERVE THESE STYLES IN THE HEAD\r\n      ------------------------------------- */\r\n      @media all {\r\n        .ExternalClass {\r\n          width: 100%; \r\n        }\r\n        .ExternalClass,\r\n        .ExternalClass p,\r\n        .ExternalClass span,\r\n        .ExternalClass font,\r\n        .ExternalClass td,\r\n        .ExternalClass div {\r\n          line-height: 100%; \r\n        }\r\n        .apple-link a {\r\n          color: inherit !important;\r\n          font-family: inherit !important;\r\n          font-size: inherit !important;\r\n          font-weight: inherit !important;\r\n          line-height: inherit !important;\r\n          text-decoration: none !important; \r\n        }\r\n        #MessageViewBody a {\r\n          color: inherit;\r\n          text-decoration: none;\r\n          font-size: inherit;\r\n          font-family: inherit;\r\n          font-weight: inherit;\r\n          line-height: inherit;\r\n        }\r\n        .btn-primary table td:hover {\r\n          background-color: #34495e !important; \r\n        }\r\n        .btn-primary a:hover {\r\n          background-color: #34495e !important;\r\n          border-color: #34495e !important; \r\n        } \r\n      }\r\n\r\n    </style>\r\n  </head>\r\n  <body>\r\n    <span class=\"preheader\">This is preheader text. Some clients will show this text as a preview.</span>\r\n    <table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"body\">\r\n      <tr>\r\n        <td>&nbsp;</td>\r\n        <td class=\"container\">\r\n          <div class=\"content\">\r\n\r\n            <!-- START CENTERED WHITE CONTAINER -->\r\n            <table role=\"presentation\" class=\"main\">\r\n\r\n              <!-- START MAIN CONTENT AREA -->\r\n              <tr>\r\n                <td class=\"wrapper\">\r\n                  <table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">\r\n                    <tr>\r\n                      <td>\r\n                        <p>Hi there,</p>\r\n                        <p>Sometimes you just want to send a simple HTML email with a simple design and clear call to action. This is it.</p>\r\n                        <table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"btn btn-primary\">\r\n                          <tbody>\r\n                            <tr>\r\n                              <td align=\"left\">\r\n                                <table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">\r\n                                  <tbody>\r\n                                    <tr>\r\n                                      <td> <a href=\"http://htmlemail.io\" target=\"_blank\">Call To Action</a> </td>\r\n                                    </tr>\r\n                                  </tbody>\r\n                                </table>\r\n                              </td>\r\n                            </tr>\r\n                          </tbody>\r\n                        </table>\r\n                        <p>This is a really simple email template. Its sole purpose is to get the recipient to click the button with no distractions.</p>\r\n                        <p>Good luck! Hope it works.</p>\r\n                      </td>\r\n                    </tr>\r\n                  </table>\r\n                </td>\r\n              </tr>\r\n\r\n            <!-- END MAIN CONTENT AREA -->\r\n            </table>\r\n            <!-- END CENTERED WHITE CONTAINER -->\r\n\r\n            <!-- START FOOTER -->\r\n            <div class=\"footer\">\r\n              <table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">\r\n                <tr>\r\n                  <td class=\"content-block\">\r\n                    <span class=\"apple-link\">Company Inc, 3 Abbey Road, San Francisco CA 94102</span>\r\n                    <br> Don't like these emails? <a href=\"http://i.imgur.com/CScmqnj.gif\">Unsubscribe</a>.\r\n                  </td>\r\n                </tr>\r\n                <tr>\r\n                  <td class=\"content-block powered-by\">\r\n                    Powered by <a href=\"http://htmlemail.io\">HTMLemail</a>.\r\n                  </td>\r\n                </tr>\r\n              </table>\r\n            </div>\r\n            <!-- END FOOTER -->\r\n\r\n          </div>\r\n        </td>\r\n        <td>&nbsp;</td>\r\n      </tr>\r\n    </table>\r\n  </body>\r\n</html>"
                    };

                    using var smtp = new SmtpClient();
                    smtp.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
                    //smtp.Authenticate(userEmail,""); password and email to send mail
                    smtp.Send(message);
                    smtp.Disconnect(true);

                }
            }
            return RedirectToAction("Participant", new {id=id});
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

                        userId = item.UserId,
                        
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
