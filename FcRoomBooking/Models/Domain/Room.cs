using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FcRoomBooking.Models.Domain
{
    public class Room
    {
        [Key]
        public int Id { get; set; }
        public string? RoomName { get; set; }
        public string? RoomCapacity { get; set; }
        public string? RoomDescription { get; set; }
        public byte[]? RoomImage { get; set; }
        [ForeignKey("RoomStatus")]
        public int? RoomStatusId { get; set; }
        public RoomStatus? RoomStatus { get; set; }
        [NotMapped]
        public IFormFile? image { get; set; }

    }
}
