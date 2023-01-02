using FcRoomBooking.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FcRoomBooking.Models.Domain
{
    public class Email
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Subject { get; set; }
        public string? Description { get; set; }

        [ForeignKey("ApplicationUser")]
        public string? ByUserID { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public DateTime? SendDate = DateTime.Now;
        [NotMapped]
        public List<AddUser>? addUser { get; set; }
    }
}
