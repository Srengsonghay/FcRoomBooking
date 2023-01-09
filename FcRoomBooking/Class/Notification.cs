using FcRoomBooking.Areas.Identity.Data;
using FcRoomBooking.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Specialized;
using System.ComponentModel;

namespace FcRoomBooking.Class
{
    public class Notification : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;


        public Notification(ApplicationDbContext dbContext,UserManager<ApplicationUser> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }
        public List<NotificationViewModel> Noti(string id)
        {
            var noti = dbContext.Participants.Where(x=>x.UserId == id).Include(x => x.RoomBooking).Include(x=>x.RoomBooking.Room).Where(x=>x.RoomBooking.BookingStatus=="Active").ToList();
            List<NotificationViewModel> notificationList = new List<NotificationViewModel>();
            if(noti.Any())
            {
                foreach (var item in noti)
                {
                    notificationList.Add(new NotificationViewModel
                    {
                        subject = item.RoomBooking.Subject,
                        detail = item.RoomBooking.Detail,
                        bookingFrom = item.RoomBooking.BookingFrom.ToString("dd/MM/yy HH:mm tt"),
                        bookingTo = item.RoomBooking.BookingTo,
                        roomNumber = item.RoomBooking.Room.RoomName,
                        id= item.RoomBooking.Id,

                    });
                }
                return notificationList;

            }
            else
            {
                return null;
            }
        }
    }
}
