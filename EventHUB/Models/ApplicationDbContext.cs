using EventHUB.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventHUB.Models
{
    public class ApplicationDbContext : Controller
    {
        public class ApplicationDbcontext : DbContext
        {
            public ApplicationDbcontext(DbContextOptions<ApplicationDbcontext> options) : base(options) { }
            public DbSet<User> Users { get; set; }
            public DbSet<Event> Events { get; set; }
            public DbSet<Register> Registers { get; set; }
            public DbSet<UserFeedback> Feedbacks { get; set; }
            public DbSet<Meeting> Meetings { get; set; }
            public DbSet<EventMedia> EventMedias { get; set; }
            public DbSet<Financial> Financials { get; set; }
            public DbSet<AdminNotification> AdminNotifications { get; set; }
            public DbSet<MemberMessage> MemberMessages { get; set; }

        }
    }

}