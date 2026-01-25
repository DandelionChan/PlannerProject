using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public class TaskActivity : UserActivity
    {
        public DateOnly DueDate { get; set; }
        public bool IsCompleted { get; set; }

        protected TaskActivity() { }
        public TaskActivity(
            string name,
            string userId,
            DateOnly date,
            string description,
            string color,
            RecurrenceType recurrence,
            DateOnly dueDate)
            : base(name, userId, date, description, color, recurrence)
        {
            DueDate = dueDate;
            IsCompleted = false;
        }
        public void MarkAsCompleted()
        {
            IsCompleted = true;
        }
        public override string GetActivityType()
        {
            return "Task";
        }
    }

}
