using Departments.Application.Departments;
using Departments.Application.Reminders;
using Departments.Presistence;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Departments
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddDbContext<ApplicationDbContext>(
                options => options
                    .UseSqlServer(builder.Configuration.GetConnectionString("Default")));

            ConfigureServices(builder.Services);

            builder.Services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));

            builder.Services.AddHangfireServer();


            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseHangfireDashboard();

            app.UseAuthorization();


            app.MapControllers();
            app.MapHangfireDashboard();
            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<DepartmentService>();
            services.AddScoped<ReminderService>();
            services.AddSingleton<IMemoryCache, MemoryCache>();
        }
    }
}
