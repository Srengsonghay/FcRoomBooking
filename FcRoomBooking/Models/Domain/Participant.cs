using FcRoomBooking.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FcRoomBooking.Models.Domain
{
    public class Participant
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("ApplicationUser")]
        public string? UserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        [ForeignKey("RoomBooking")]
        public int RoomBookingId { get; set; }
        public RoomBooking? RoomBooking { get; set; }
        public string? Reason { get; set; }
        public bool? IsExcept { get; set; }
        
    }
}
