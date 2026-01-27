using BusinessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Contexts
{
    public class TaskActivityContext: IDB<TaskActivity, int>
    {
        private readonly PlannerDbContext context;

        public TaskActivityContext(PlannerDbContext context)
        {
            this.context = context;
        }

        public async Task CreateAsync(TaskActivity item)
        {
            try
            {
                context.Set<TaskActivity>().Add(item);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TaskActivity> ReadAsync(int key)
        {
            try
            {
                return await context.Activities.OfType<TaskActivity>()
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.ActivityId == key);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<TaskActivity>> ReadAllAsync()
        {
            try
            {
                return await context.Set<TaskActivity>()
                    .Include(t => t.User)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateAsync(TaskActivity item)
        {
            try
            {
                context.Set<TaskActivity>().Update(item);
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
                TaskActivity task = await context.Set<TaskActivity>().FindAsync(key);

                if (task == null)
                {
                    throw new ArgumentException("Task does not exist!");
                }

                context.Set<TaskActivity>().Remove(task);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
