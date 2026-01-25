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
                    .FirstOrDefaultAsync(r => r.Id == key);
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
                context.DailyRemiders.Update(item);
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
                DailyRemider reminder = await context.DailyRemiders.FindAsync(key);

                if (reminder == null)
                {
                    throw new ArgumentException("Reminder does not exist!");
                }

                context.DailyRemiders.Remove(reminder);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
