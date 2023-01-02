using FcRoomBooking.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace FcRoomBooking.Models.ViewModel
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<ApplicationRole> Roles { get; set; }
        
    }
}
