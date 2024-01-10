using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Test.Me.models;

namespace Test.Me.Pages
{
    public class NewExamModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<NewExamModel> _logger;
        private readonly ApplicationDbContext _context;

        public NewExamModel(IConfiguration configuration, ILogger<NewExamModel> logger, ApplicationDbContext context)
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost(string name, string date, string time)
        {
            try
            {
                // Validate the input
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(date) || string.IsNullOrWhiteSpace(time))
                {
                    ModelState.AddModelError("", "Exam name, date, and time are required.");
                    return Page();
                }

                // Convert date and time to DateTime
                DateTime examDate;
                if (!DateTime.TryParse(date, out examDate))
                {
                    ModelState.AddModelError("", "Invalid date format. Use YYYY-MM-DD.");
                    return Page();
                }

                int examTime;
                if (!int.TryParse(time, out examTime) || examTime <= 0)
                {
                    ModelState.AddModelError("", "Invalid exam time. Must be a positive integer.");
                    return Page();
                }

                // Get TeacherId from session
                var teacherId = HttpContext.Session.GetInt32("TeacherId");

                if (teacherId != null)
                {
                    // Create new exam and save it to the database
                    var newExam = new Quiz
                    {
                        Tid = teacherId.Value,
                        Qname = name,
                        Qdate = examDate,
                        Qtime = examTime
                    };

                    _context.Quiz.Add(newExam);
                    _context.SaveChanges();

                    // Store the newly created quiz ID in session for later reference
                    HttpContext.Session.SetInt32("Qid", newExam.Qid);

                    return RedirectToPage("/Teachers/AddQuestion");
                }
                else
                {
                    // Teacher not logged in
                    return RedirectToPage("/Teachers/Login");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating a new exam.");
                ModelState.AddModelError("", "An error occurred. Please try again.");
                return Page();
            }
        }
    }
}
