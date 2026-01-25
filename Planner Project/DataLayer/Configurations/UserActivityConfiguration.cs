using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;   
using BusinessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Configurations
{
 
        public class UserActivityConfiguration : IEntityTypeConfiguration<UserActivity>
        {
            public void Configure(EntityTypeBuilder<UserActivity> builder)
            {
                builder.HasKey(a => a.ActivityId);
                builder.HasIndex(a => a.UserId);

            builder.HasOne(ua => ua.User)
                   .WithMany(u => u.UserActivities)
                   .HasForeignKey(ua => ua.UserId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }

