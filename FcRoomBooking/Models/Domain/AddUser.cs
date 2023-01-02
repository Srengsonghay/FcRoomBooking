using FcRoomBooking.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FcRoomBooking.Models.Domain
{
    public class AddUser
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("EmailSender")]
        public int? EmailId { get; set; }
        public Email? Email { get; set; }
        [ForeignKey("ApplicationUser")]
        public string? UserID { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
