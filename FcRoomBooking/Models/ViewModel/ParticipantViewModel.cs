using FcRoomBooking.Models.Domain;

namespace FcRoomBooking.Models.ViewModel
{
    public class ParticipantViewModel
    {
        public string[] ParticipantId { get; set; }
        public int Id { get; set; }
        public int RoomBookingId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string? userId { get; set; }
        public string? Subject { get; set; }
        public string? Detail { get; set; }
        
        public string? Reason { get; set; }
        public bool? IsExcept { get; set; }

    }
}
