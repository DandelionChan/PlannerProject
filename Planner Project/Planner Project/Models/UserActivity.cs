using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public abstract class UserActivity : Activity
    {
        public string UserId { get; set; }
        public User User { get; set; }

        public UserActivity() { }
        public UserActivity(
            string name,
            string userId,
            DateOnly date,
            string description,
            string color,
            RecurrenceType recurrence)
            : base(name, date, description, color, recurrence)
        {
            UserId = userId;
        }

        public override string GetActivityType()
        {
            return "USerActivity";
        }

    }
}
