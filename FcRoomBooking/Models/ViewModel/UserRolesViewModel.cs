namespace FcRoomBooking.Models.ViewModel
{
    
        public class ManageUserRolesViewModel
        {
            public string UserId { get; set; }
            public string Email { get; set; }
            public IList<UserRolesViewModel>? UserRoles { get; set; }
        }
        public class UserRolesViewModel
        {
            public string? RoleName { get; set; }
            public bool Selected { get; set; }
        }
    
}
