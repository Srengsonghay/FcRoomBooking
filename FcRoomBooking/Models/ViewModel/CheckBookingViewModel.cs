using FcRoomBooking.Models.Domain;

namespace FcRoomBooking.Models.ViewModel
{
    public class CheckBookingViewModel
    {
        public List<RoomBooking>? RoomBooking { get; set; }
        public List<Participant>? Participant { get; set; }
       
    }
}
