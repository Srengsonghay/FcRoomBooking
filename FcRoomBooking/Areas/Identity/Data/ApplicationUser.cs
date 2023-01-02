using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace FcRoomBooking.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [Display(Name = "Username")]
    public string? Name { get; set; }
    [PersonalData]
    public Byte[]? ProfileImage { get; set; }
    [NotMapped]
    public IFormFile? Image { get; set; }
}

