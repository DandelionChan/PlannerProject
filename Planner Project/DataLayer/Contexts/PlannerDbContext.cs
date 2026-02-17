using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Models;
using DataLayer.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Contexts
{
    public class PlannerDbContext: IdentityDbContext<User>
    {
        public PlannerDbContext(DbContextOptions<PlannerDbContext> options)
        : base(options)
        {

        }

        public DbSet<Activity> Activities { get; set; }
        public DbSet<DailyRemider> DailyRemiders { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(ActivityConfiguration).Assembly);
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new DailyRemindersConfiguration());

            //Seed data for DailyRemiders
            builder.Entity<DailyRemider>().HasData(
    new DailyRemider { DailyRemiderId = 1, Text = "Drink 2L Water", Recurrence = DailyRemider.RecurrenceType.Daily },
    new DailyRemider { DailyRemiderId = 2, Text = "Morning Meditation", Recurrence = DailyRemider.RecurrenceType.Daily },
    new DailyRemider { DailyRemiderId = 3, Text = "Read 10 Pages", Recurrence = DailyRemider.RecurrenceType.Daily },
    new DailyRemider { DailyRemiderId = 4, Text = "Evening Walk", Recurrence = DailyRemider.RecurrenceType.Daily },
    new DailyRemider { DailyRemiderId = 6, Text = "Practice Coding for 30 Minutes", Recurrence = DailyRemider.RecurrenceType.Daily }
);

        }
    }
}
