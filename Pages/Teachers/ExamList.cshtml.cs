using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Test.Me.models;

namespace Test.Me.Pages.Teachers
{
    public class ExamListModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ExamListModel> _logger;
        private readonly ApplicationDbContext _context;

        public List<Quiz> Exams { get; set; }

        public ExamListModel(IConfiguration configuration, ILogger<ExamListModel> logger, ApplicationDbContext context)
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;
            Exams = new List<Quiz>();
        }

        public IActionResult OnGet()
        {
            // Retrieve teacher ID from session
            var teacherId = HttpContext.Session.GetInt32("TeacherId");

            if (teacherId == null)
            {
                return RedirectToPage("/Login");
            }

            // Retrieve exams for the teacher
            Exams = _context.Quiz
                .Where(q => q.Tid == teacherId)
                .ToList();

            return Page();
        }

        public IActionResult OnPostDeleteExam(int id)
        {
            var teacherId = HttpContext.Session.GetInt32("TeacherId");


            var exam = _context.Quiz
                .Include(q => q.Questions)
                .FirstOrDefault(q => q.Qid == id && q.Tid == teacherId);

            // Retrieve questions of the exam
            var examQuestions = _context.Question
                .Where(q => q.Qid == id)
                .ToList();

            // Delete associated records in Student_Question table
            var questionIds = _context.Question
                .Where(q => q.Qid == exam.Qid)
                .Select(q => q.Quid)
                .ToList();
            var studentQuestions = _context.Student_Question
                .Where(sq => questionIds.Contains(sq.Quid))
                .ToList();

            // Remove the associated records from the context
            _context.Student_Question.RemoveRange(studentQuestions);

            // Delete associated records in Student_Quiz table
            var studentQuizzes = _context.Student_Quiz
                .Where(sq => sq.Qid == exam.Qid)
                .ToList();
            _context.Student_Quiz.RemoveRange(studentQuizzes);

            // Remove questions from the database
            _context.Question.RemoveRange(examQuestions);

            // Remove the exam
            _context.Quiz.Remove(exam);

            // Save changes to the database
            _context.SaveChanges();

            return RedirectToPage("/Teachers/ExamList");
        }

        public IActionResult OnPostAddQuestions(int id)
        {
            return RedirectToPage("/Teachers/AddQuestion", new { qid = id });
        }

        public IActionResult OnPostStudentsPerformance(int id)
        {
            return RedirectToPage("/Teachers/StudentsPerformance", new { qid = id });
        }
    }
}
