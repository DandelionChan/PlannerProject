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
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<TaskActivity> Tasks { get; set; }
        public DbSet<AppointmentActivity> Appointments { get; set; }
        public DbSet<BirthdayActivity> Birthdays { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<DailyRemider> DailyRemiders { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(ActivityConfiguration).Assembly);
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new DailyRemindersConfiguration());
            base.OnModelCreating(builder);
        }
    }
}
