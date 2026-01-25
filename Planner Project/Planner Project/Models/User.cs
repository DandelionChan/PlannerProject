using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<UserActivity> UserActivities { get; set; }
        public ICollection<DailyRemider> DailyRemiders { get; set; }

        public User() { }

        public User(string userName, string email, string firstName, string lastName, List<UserActivity> userActivities = null,
        List<DailyRemider> dailyRemiders = null)
        {
            UserName = userName;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            UserActivities = userActivities ?? new List<UserActivity>();
            DailyRemiders = dailyRemiders ?? new List<DailyRemider>();
        }
    }
}
