using Microsoft.AspNetCore.Identity;

namespace FcRoomBooking.Areas.Identity.Data
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole()
        {

        }
        public ApplicationRole(string roleName) : base(roleName)
        {
        }
        
        
        public bool? Islocked { get; set; }
    }
}
