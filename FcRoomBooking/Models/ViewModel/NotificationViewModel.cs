namespace FcRoomBooking.Models.ViewModel
{
    public class NotificationViewModel
    {
        public string subject { get; set; }
        public string detail { get; set; }
        public string? bookingFrom { get; set; }
        public DateTime? bookingTo { get; set; }
        public string roomNumber { get; set; }
        public int id { get; set; }
    }
}
