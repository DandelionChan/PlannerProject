using BusinessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Contexts
{
    public class HolidayContext: IDB<Holiday, int>
    {
        private readonly PlannerDbContext context;

        public HolidayContext(PlannerDbContext context)
        {
            this.context = context;
        }

        public async Task CreateAsync(Holiday item)
        {
            context.Set<Holiday>().Add(item);
            await context.SaveChangesAsync();
        }

        public async Task<Holiday> ReadAsync(int key)
        {
            return await context.Set<Holiday>().FirstOrDefaultAsync(h => h.ActivityId == key);
        }

        public async Task<IEnumerable<Holiday>> ReadAllAsync()
        {
            return await context.Set<Holiday>().OrderBy(h => h.Date).ToListAsync();
        }

        public async Task UpdateAsync(Holiday item)
        {
            context.Set<Holiday>().Update(item);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int key)
        {
            var holiday = await context.Set<Holiday>().FindAsync(key);

            if (holiday == null)
                throw new ArgumentException("Holiday not found.");

            context.Set<Holiday>().Remove(holiday);
            await context.SaveChangesAsync();
        }

        public async Task<bool> HolidayExistsAsync(string name, DateOnly date)
        {
            return await context.Set<Holiday>().AnyAsync(h =>
                h.Name == name && h.Date == date);
        }
    }
}
