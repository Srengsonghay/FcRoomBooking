using FcRoomBooking.Areas.Identity.Data;
using FcRoomBooking.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FcRoomBooking.Areas.Identity.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<RoomBooking> RoomBookings { get; set; }
    public DbSet<Participant> Participants { get; set; }
    public DbSet<RoomStatus> RoomStatus { get; set; }
    public DbSet<ApplicationRole> ApplicationRole { get; set; }
    public DbSet<ApplicationUser> ApplicationUser { get; set; }
    public DbSet<Email> Email { get; set; }
    public DbSet<AddUser> AddUser { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        //builder.Entity<Participant>()
        //    .HasKey(t => new { t.UserId, t.RoomBookingId });
        //builder.Entity<Participant>()
        //    .HasOne(pt => pt.RoomBooking)
        //    .WithMany(p => p.Participant)
        //    .HasForeignKey(p => p.RoomBookingId);

        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
