using BusinessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Contexts
{
    public class AppointmentActivityContext: IDB<AppointmentActivity, int>
    {
        private readonly PlannerDbContext context;

        public AppointmentActivityContext(PlannerDbContext context)
        {
            this.context = context;
        }

        public async Task CreateAsync(AppointmentActivity item)
        {
            if (await HasOverlappingAppointmentAsync(
                item.UserId, item.Date, item.StartTime, item.EndTime))
            {
                throw new InvalidOperationException("Appointment overlaps with an existing one.");
            }

            context.Set<AppointmentActivity>().Add(item);
            await context.SaveChangesAsync();
        }

        public async Task<AppointmentActivity> ReadAsync(int key)
        {
            return await context.Set<AppointmentActivity>()
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.ActivityId == key);
        }

        public async Task<IEnumerable<AppointmentActivity>> ReadAllAsync()
        {
            return await context.Set<AppointmentActivity>()
                .Include(a => a.User)
                .ToListAsync();
        }

        public async Task UpdateAsync(AppointmentActivity item)
        {
            if (await HasOverlappingAppointmentAsync(
                item.UserId, item.Date, item.StartTime, item.EndTime, item.ActivityId))
            {
                throw new InvalidOperationException("Appointment overlaps with an existing one.");
            }

            context.Set<AppointmentActivity>().Update(item);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int key)
        {
            var appointment = await context.Set<AppointmentActivity>().FindAsync(key);

            if (appointment == null)
                throw new ArgumentException("Appointment not found.");

            context.Set<AppointmentActivity>().Remove(appointment);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AppointmentActivity>> GetAppointmentsByDateAsync(DateOnly date, string userId)
        {
            return await context.Set<AppointmentActivity>()
                .Where(a => a.Date == date && a.UserId == userId)
                .OrderBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<bool> HasOverlappingAppointmentAsync(string userId, DateOnly date, TimeOnly start,
            TimeOnly end, int? excludeAppointmentId = null)
        {
            return await context.Set<AppointmentActivity>().AnyAsync(a =>
                a.UserId == userId &&
                a.Date == date &&
                (excludeAppointmentId == null || a.ActivityId != excludeAppointmentId) &&
                start < a.EndTime &&
                end > a.StartTime
            );
        }
    }
}
