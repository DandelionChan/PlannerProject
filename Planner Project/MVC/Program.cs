using BusinessLayer.Models;
using DataLayer.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Add Services
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages(); // Required if you use Identity UI (Scaffolded)

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";

                options.Events.OnRedirectToReturnUrl = context =>
                {
                    // Check if they are being sent to the root "/" or if it's empty
                    if (context.RedirectUri == "/" || string.IsNullOrEmpty(context.RedirectUri))
                    {
                        context.Response.Redirect("/Calendar");
                    }
                    else
                    {
                        // Otherwise, let them go where they were headed
                        context.Response.Redirect(context.RedirectUri);
                    }
                    return Task.CompletedTask;
                };
            });

            // 2. Database Configuration
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<PlannerDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // 3. Identity Configuration (Use only ONE of these)
            builder.Services.AddIdentity<User, IdentityRole>(options => {
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<PlannerDbContext>()
            .AddDefaultTokenProviders()
            .AddDefaultUI(); // This enables the default login/register pages



            //controllers
            builder.Services.AddScoped<ActivityContext>();
            builder.Services.AddScoped<AppointmentActivityContext>();
            builder.Services.AddScoped<DailyReminderContext>();
            builder.Services.AddScoped<HolidayContext>();
            builder.Services.AddScoped<TaskActivityContext>();

            var app = builder.Build();

            // 4. Middleware Pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // Must come before Authorization
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages(); // Required for Identity default UI to work

            app.Run();
        }
    }
}
