using StudentMvcApp.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using StudentMvcApp.Services;

namespace StudentMvcApp.Job
{
    public class MyJob
    {
        private readonly ILogger<MyJob> _logger;
        private readonly StudentDbContext _context;
        private readonly IEmailSender _emailSender; 

        public MyJob(ILogger<MyJob> logger, StudentDbContext context, IEmailSender emailSender)
        {
            _logger = logger;
            _context = context;
            _emailSender = emailSender;
        }

        public void RunTask()
        {
            int studentCount = _context.Students.Count(s => !s.IsDeleted);
            string message = $"[10-Minute Report] - Total Active Students: {studentCount} - {DateTime.Now}";

            _logger.LogInformation(message);
            Console.WriteLine(message);

            try
            {
                _emailSender.send(
                    to: "meetparmar7156@gmail.com",
                    subject: "Student Report",
                    body: message
                );

                _logger.LogInformation("Email sent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send report email.");
            }
        }
    }
}
