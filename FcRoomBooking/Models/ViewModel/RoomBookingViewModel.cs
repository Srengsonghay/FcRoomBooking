using FcRoomBooking.Areas.Identity.Data;
using FcRoomBooking.Models.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace FcRoomBooking.Models.ViewModel
{
    public class RoomBookingViewModel
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? RoomName { get; set; }
        public string? Subject { get; set; }
        public string? Detail { get; set; }
        public string? BookingFrom { get; set; }
        public string? BookingTo { get; set; }
        public string? BookingStatus { get; set; }
        public string? BookingFromTime { get; set; }
        public string? BookingToTime { get; set; }
        public List<ParticipantViewModel>? Participants { get; set; }
        public ParticipantViewModel? participantViewModel { get; set; }

    }
}
