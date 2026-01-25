using BusinessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Configurations
{
    public class DailyRemindersConfiguration: IEntityTypeConfiguration<DailyRemider>
    {
        public void Configure(EntityTypeBuilder<DailyRemider> builder)
        {
            builder.ToTable("DailyRemiders");

            builder.HasKey(dr => dr.DailyRemiderId);

            builder.Property(dr => dr.Text)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(dr => dr.Recurrence)
                   .IsRequired();

            builder
                .HasMany(dr => dr.Users)
                .WithMany(u => u.DailyRemiders)
                .UsingEntity<Dictionary<string, object>>(
                    "UserDailyRemider",
                    j => j
                        .HasOne<User>()
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_UserDailyRemider_User")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<DailyRemider>()
                        .WithMany()
                        .HasForeignKey("DailyRemiderId")
                        .HasConstraintName("FK_UserDailyRemider_DailyRemider")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("UserId", "DailyRemiderId");
                        j.ToTable("UserDailyRemiders");
                    });
        }

    }
}
