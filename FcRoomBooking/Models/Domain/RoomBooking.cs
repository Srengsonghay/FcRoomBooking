using FcRoomBooking.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FcRoomBooking.Models.Domain
{
    public class RoomBooking
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Room")]
        public int RoomId { get; set; }
        public Room? Room { get; set; }
        [ForeignKey("ApplicationUser")]
        public string? UserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public string Subject { get; set; }
        public string? Detail { get; set; }
        public DateTime BookingFrom { get; set; }
        public DateTime BookingTo { get; set; }
        public string? BookingStatus { get; set; }
        [NotMapped]
        public List<Participant> Participant { get; set; }

    }
}
