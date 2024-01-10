using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Test.Me.models;

namespace Test.Me.Pages.Students
{
    public class ExamsMarksModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ExamsMarksModel> _logger;
        private readonly ApplicationDbContext _context;

        public List<ExamMarkViewModel> ExamsMarks { get; set; }

        public ExamsMarksModel(IConfiguration configuration, ILogger<ExamsMarksModel> logger, ApplicationDbContext context)
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

        public IActionResult OnGet()
        {
            var currentStudent = GetCurrentStudent();

            if (currentStudent == null)
            {
                return RedirectToPage("/Login");
            }

            ExamsMarks = _context.Student_Quiz
                .Where(sq => sq.Sid == currentStudent.Sid)
                .Select(sq => new ExamMarkViewModel
                {
                    ExamName = sq.Quiz.Qname,
                    Mark = sq.Smark
                })
                .ToList();

            return Page();
        }

        private Student GetCurrentStudent()
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");

            if (studentId != null)
            {
                return _context.Students.FirstOrDefault(s => s.Sid == studentId);
            }

            return null;
        }
    }

    public class ExamMarkViewModel
    {
        public string ExamName { get; set; }
        public int Mark { get; set; }
    }
}
