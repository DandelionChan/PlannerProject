using BusinessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Contexts
{
    public class ActivityContext: IDB<Activity, int>
    {
        private PlannerDbContext context;

        public ActivityContext(PlannerDbContext context)
        {
            this.context = context;
        }

        public async Task CreateAsync(Activity item)
        {
            context.Activities.Add(item);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Activity>> ReadAllAsync()
            => await context.Activities.ToListAsync();

        public async Task<Activity> ReadAsync(int key)
            => await context.Activities.FindAsync(key);

        public async Task UpdateAsync(Activity item)
        {
            context.Update(item);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int key)
        {
            var entity = await ReadAsync(key);
            context.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}
