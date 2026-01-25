using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public class BirthdayActivity : UserActivity
    {
        public string BirthdayPerson { get; set; }

        protected BirthdayActivity() { }
        public BirthdayActivity(
            string name,
            string userId,
            string birthdayPerson,
            DateOnly date,
            string description,
            string color,
            RecurrenceType recurrence = RecurrenceType.Yearly)
            : base(name, userId, date, description, color, recurrence)
        {
            BirthdayPerson = birthdayPerson;
        }

        public override string GetActivityType()
        {
            return "Birthday";
        }
    }
}
