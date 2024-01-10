using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Test.Me.models;

namespace Test.Me.Pages.Students
{
    public class StudentsPerformanceModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<StudentsPerformanceModel> _logger;
        private readonly ApplicationDbContext _context;

        [BindProperty(SupportsGet = true)]
        public int? Qid { get; set; }

        public Quiz Exam { get; set; }

        public List<StudentPerformanceViewModel> StudentInfoList { get; set; }

        public StudentsPerformanceModel(IConfiguration configuration, ILogger<StudentsPerformanceModel> logger, ApplicationDbContext context)
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;
            StudentInfoList = new List<StudentPerformanceViewModel>();
        }

        public IActionResult OnGet(int? qid)
        {
            // Check if Qid is not provided in the query parameters
            if (qid == null)
            {
                // Redirect to the Index1 page if Qid is not provided
                return RedirectToPage("/Index1");
            }

            // Retrieve the exam details based on Qid
            Exam = _context.Quiz.Find(qid);

            // Check if the exam is not found
            if (Exam == null)
            {
                // Redirect to the Index2 page if the exam is not found
                return RedirectToPage("/Index2");
            }

            // Retrieve student information for the exam
            StudentInfoList = _context.Student_Quiz
                .Where(sq => sq.Qid == qid)
                .Select(sq => new StudentPerformanceViewModel
                {
                    Sid = sq.Sid,
                    StudentName = _context.Students.FirstOrDefault(s => s.Sid == sq.Sid).Sname,
                    Mark = sq.Smark
                })
                .ToList();

            return Page();
        }
    }

    public class StudentPerformanceViewModel
    {
        public int Sid { get; set; }
        public string StudentName { get; set; }
        public int Mark { get; set; }
    }
}
