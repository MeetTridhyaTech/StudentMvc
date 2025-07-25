using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using StudentMvcApp.Data;
using StudentMvcApp.Validators;
using Hangfire;
using StudentMvcApp.Job;
using Microsoft.AspNetCore.Identity.UI.Services;
using StudentMvcApp.Services;
namespace StudentMvcApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            // Add services to the container.

            builder.Services.AddControllersWithViews()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<StudentValidator>());


            builder.Services.AddDbContext<StudentDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Hangfire configuration
            builder.Services.AddHangfire(config =>
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseRecommendedSerializerSettings()
                        .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<StudentMvcApp.Services.IEmailSender, EmailSender>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                //app.UseExceptionHandler("/Home/Error");
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            RecurringJob.AddOrUpdate<MyJob>(
                "report-every-1-minutes",
                job => job.RunTask(),
                "0 * * * *"
                //"* * * * *"
                //Cron.Minutely
);
            
            app.UseStaticFiles();
            app.UseRouting();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
