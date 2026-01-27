using BusinessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Configurations
{
    public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
    {
        public void Configure(EntityTypeBuilder<Activity> builder)
        {
            builder.ToTable("Activities");
            builder.HasKey(a => a.ActivityId);
            //builder.HasIndex(a => a.UserId);
            //builder.Property(a => a.UserId)
            //       .IsRequired();

            //builder.HasOne(a => a.User)
            //       .WithMany(u => u.Activities)
            //       .HasForeignKey(a => a.UserId)
            //       .OnDelete(DeleteBehavior.Cascade);

            builder.Property(a => a.Name).IsRequired().HasMaxLength(200);
            builder.Property(a => a.Color).IsRequired().HasMaxLength(50);
            builder.Property(a => a.Recurrence).IsRequired();
            builder.Property(a => a.Date)
                   .HasConversion(
                    d => d.ToDateTime(TimeOnly.MinValue),
                    d => DateOnly.FromDateTime(d))
                   .HasColumnType("date");

            // TPH discriminator for inheritance
            builder.HasDiscriminator<string>("ActivityDiscriminator")
              .HasValue<Holiday>("Holiday")
              .HasValue<UserActivity>("UserActivity") // abstract base for user-bound activities
              .HasValue<AppointmentActivity>("Appointment")
              .HasValue<TaskActivity>("Task")
              .HasValue<BirthdayActivity>("Birthday");
        }
    }
}
