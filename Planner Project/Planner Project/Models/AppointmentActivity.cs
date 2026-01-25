using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public class AppointmentActivity : UserActivity
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        protected AppointmentActivity() { }
        public AppointmentActivity(
            string name,
            string userId,
            DateOnly date,
            string description,
            string color,
            RecurrenceType recurrence,
            TimeOnly startTime,
            TimeOnly endTime)
            : base(name, userId, date, description, color, recurrence)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        public void ValidateTime()
        {
            if (EndTime < StartTime)
            {
                throw new InvalidOperationException("End time cannot be earlier than start time.");
            }
        }
        public override string GetActivityType()
        {
            return "Appointment";
        }
    }
}
