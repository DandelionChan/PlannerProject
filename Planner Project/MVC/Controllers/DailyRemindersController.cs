using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BusinessLayer.Models;
using DataLayer.Contexts;
using System.Security.Claims;

namespace MVC.Controllers
    {
    [Authorize]
    public class DailyRemindersController : Controller
    {
        private readonly DailyReminderContext _reminderContext;

        public DailyRemindersController(DailyReminderContext reminderContext)
        {
            _reminderContext = reminderContext;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var myReminders = await _reminderContext.GetRemindersForUser(userId);

            return View(myReminders);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string text, DailyRemider.RecurrenceType recurrence)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var newReminder = new DailyRemider(text, recurrence);

                try
                {
                    await _reminderContext.CreateAsync(newReminder);

                    await _reminderContext.AssignReminderToUser(newReminder.DailyRemiderId, userId);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Creation error: " + ex.Message);
                }
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Unsubscribe(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _reminderContext.RemoveReminderFromUser(id, userId);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _reminderContext.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<JsonResult> GetAllForManagement()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var allReminders = await _reminderContext.ReadAllAsync();
            var myReminders = await _reminderContext.GetRemindersForUser(userId);

            var result = allReminders.Select(r => new {
                id = r.DailyRemiderId,
                text = r.Text,
                isActive = myReminders.Any(my => my.DailyRemiderId == r.DailyRemiderId)
            });

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _reminderContext.AssignReminderToUser(id, userId);
            return Ok();
        }
    }
}
