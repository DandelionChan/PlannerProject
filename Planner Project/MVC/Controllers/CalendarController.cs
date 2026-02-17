using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using BusinessLayer.Models;
using DataLayer.Contexts;
using Activity = BusinessLayer.Models.Activity; 

namespace MVC.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
    private readonly PlannerDbContext _context;
    private readonly DailyReminderContext _reminderContext;

    // FIX: Match the parameter type to your private field type
    public CalendarController(PlannerDbContext context, DailyReminderContext reminderContext)
    {
        _context = context;
        _reminderContext = reminderContext;
    }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (_context == null)
            {
                // If this hits, your constructor isn't assigning _context correctly
                return Content("Database context is not initialized.");
            }

            // Fetch user info from your User class to show a personal greeting
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            ViewData["UserName"] = user?.FirstName ?? "User";
            return View();
        }


        [HttpGet("calendar/events")]
    public async Task<IActionResult> GetEvents(DateTime start, DateTime end)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var startDate = DateOnly.FromDateTime(start);
        var endDate = DateOnly.FromDateTime(end);

        var eventList = new List<object>();



        // 1. Fetch Activities
        var activities = await _context.Activities
            .OfType<UserActivity>()
            .Where(a => a.UserId == userId)
            .ToListAsync();



        foreach (var activity in activities)
        {
            for (var day = startDate; day <= endDate; day = day.AddDays(1))
            {
                if (IsOccurringOn(activity, day))
                {
                    string startTimeStr = "00:00";
                    string endTimeStr = "01:00";

                    if (activity is AppointmentActivity appt)
                    {
                        startTimeStr = appt.StartTime.ToString("HH:mm");
                        endTimeStr = appt.EndTime.ToString("HH:mm");
                    }

                    eventList.Add(new
                    {
                        id = activity.ActivityId,
                        title = activity.Name, // Match JS expectation
                        color = activity.Color,
                        start_datetime = day.ToDateTime(TimeOnly.Parse(startTimeStr)).ToString("s"),
                        end_datetime = day.ToDateTime(TimeOnly.Parse(endTimeStr)).ToString("s"),
                        allDay = activity is not AppointmentActivity
                    });
                }
            }
        }

        // 2. Fetch Reminders (Move this OUTSIDE the activity loop)
        var userReminders = await _reminderContext.GetRemindersForUser(userId);
        foreach (var reminder in userReminders)
        {
            for (var day = startDate; day <= endDate; day = day.AddDays(1))
            {
                if (IsReminderOccurring(reminder, day))
                {
                    eventList.Add(new
                    {
                        id = "rem-" + reminder.DailyRemiderId,
                        title = "🔔 " + reminder.Text,
                        color = "#ff96e1",
                        start_datetime = day.ToDateTime(new TimeOnly(8, 0)).ToString("s"),
                        end_datetime = day.ToDateTime(new TimeOnly(9, 0)).ToString("s"),
                        allDay = true
                    });
                }
            }
        }

        return Json(eventList);
    }

        private bool IsReminderOccurring(DailyRemider reminder, DateOnly checkDate)
        {
            // Simple daily check for now, can be expanded for Weekly/Monthly
            if (reminder.Recurrence == DailyRemider.RecurrenceType.Daily) return true;
            return false;
        }
        private bool IsOccurringOn(UserActivity activity, DateOnly checkDate)
        {
            // If it happened after the check date, it can't occur
            if (activity.Date > checkDate) return false;

            // If no recurrence, it must match the date exactly
            if (activity.Recurrence == Activity.RecurrenceType.None)
                return activity.Date == checkDate;

            switch (activity.Recurrence)
            {
                case Activity.RecurrenceType.Daily:
                    return true;
                case Activity.RecurrenceType.Weekly:
                    return activity.Date.DayOfWeek == checkDate.DayOfWeek;
                case Activity.RecurrenceType.Monthly:
                    return activity.Date.Day == checkDate.Day;
                case Activity.RecurrenceType.Yearly:
                    return activity.Date.Month == checkDate.Month && activity.Date.Day == checkDate.Day;
                default:
                    return false;
            }
        }

        // ------------------------
        // POST: /calendar/events
        // ------------------------
        [HttpPost("calendar/events")]
        public async Task<IActionResult> CreateEvent([FromBody] EventDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            UserActivity activity = dto.Type switch
            {
                "task" => new TaskActivity(
                    dto.Title,
                    userId,
                    DateOnly.Parse(dto.StartDate),
                    dto.Description,
                    dto.Color,
                    Enum.TryParse<Activity.RecurrenceType>(dto.Recurrence, true, out var recType) ? recType : Activity.RecurrenceType.None,
                    DateOnly.Parse(dto.StartDate) // Pass StartDate as dueDate (required parameter)
                ),
                "appointment" => new AppointmentActivity(
                    dto.Title,
                    userId,
                    DateOnly.Parse(dto.StartDate),
                    dto.Description,
                    dto.Color,
                    Enum.TryParse<Activity.RecurrenceType>(dto.Recurrence, true, out var recType2) ? recType2 : Activity.RecurrenceType.None,
                    TimeOnly.Parse(dto.StartTime),
                    TimeOnly.Parse(dto.EndTime)
                ),
                "birthday" => new BirthdayActivity(
                    dto.Title, // BirthdayPerson
                    userId, // UserId
                    dto.Title, // Name
                    DateOnly.Parse(dto.StartDate), // Date
                    dto.Description, // Description
                    dto.Color, // Color
                    Activity.RecurrenceType.Yearly // Recurrence (birthdays are typically yearly)
                ),
                _ => null
            };

            if (activity == null) return Json(new { success = false });

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }

    // ------------------------ 
    // DTO to match JS POST payload
    // ------------------------
    public class EventDto
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public string StartDate { get; set; }       // e.g., "2026-02-14"
        public string StartTime { get; set; }       // for appointments
        public string EndTime { get; set; }         // for appointments
        public string Recurrence { get; set; }      // none, daily, yearly...
    }
}
