using BusinessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Contexts
{
    public class DailyReminderContext: IDB<DailyRemider, int>
    {
        private readonly PlannerDbContext context;

        public DailyReminderContext(PlannerDbContext context)
        {
            this.context = context;
        }

        public async Task CreateAsync(DailyRemider item)
        {
            try
            {
                context.DailyRemiders.Add(item);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DailyRemider> ReadAsync(int key)
        {
            try
            {
                return await context.DailyRemiders
                    .Include(r => r.Users)
                    .FirstOrDefaultAsync(r => r.DailyRemiderId == key);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<DailyRemider>> ReadAllAsync()
        {
            try
            {
                return await context.DailyRemiders
                    .Include(r => r.Users)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateAsync(DailyRemider item)
        {
            try
            {
                var existing = await context.DailyRemiders
                                .Include(r => r.Users)
                                .FirstOrDefaultAsync(r => r.DailyRemiderId == item.DailyRemiderId);

                if (existing == null) throw new ArgumentException("Reminder does not exist!");

                existing.Text = item.Text;
                existing.Recurrence = item.Recurrence;
                existing.Users = item.Users ?? new List<User>();

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteAsync(int key)
        {
            try
            {
                var reminder = await context.DailyRemiders
                                .Include(r => r.Users)
                                .FirstOrDefaultAsync(r => r.DailyRemiderId == key);

                if (reminder == null) throw new ArgumentException("Reminder does not exist!");

                context.DailyRemiders.Remove(reminder);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task AssignReminderToUser(int reminderId, string userId)
        {
            var reminder = await context.DailyRemiders
                .Include(r => r.Users)
                .FirstOrDefaultAsync(r => r.DailyRemiderId == reminderId);

            var user = await context.Users.FindAsync(userId);

            if (reminder == null || user == null) return;

            if (!reminder.Users.Contains(user))
                reminder.Users.Add(user);

            await context.SaveChangesAsync();
        }

        public async Task RemoveReminderFromUser(int reminderId, string userId)
        {
            var reminder = await context.DailyRemiders
                .Include(r => r.Users)
                .FirstOrDefaultAsync(r => r.DailyRemiderId == reminderId);

            var user = await context.Users.FindAsync(userId);

            if (reminder == null || user == null) return;

            if (reminder.Users.Contains(user))
                reminder.Users.Remove(user);

            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<DailyRemider>> GetRemindersForUser(string userId)
        {
            return await context.DailyRemiders
                .Include(r => r.Users)
                .Where(r => r.Users.Any(u => u.Id == userId))
                .ToListAsync();
        }

    }
}
