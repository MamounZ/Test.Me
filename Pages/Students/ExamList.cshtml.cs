using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using Test.Me.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Test.Me.Pages.Students
{
    public class ExamListModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ExamListModel> _logger;
        private readonly ApplicationDbContext _context;

        public List<(Quiz Exam, bool HasTaken)> ExamsWithTakenStatus { get; set; }

        public ExamListModel(IConfiguration configuration, ILogger<ExamListModel> logger, ApplicationDbContext context)
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

            // Retrieve exams from the database
            var exams = _context.Quiz
                .Include(e => e.Teacher)
                .Where(exam => exam.Qdate > DateTime.Now)
                .ToList();

            // Check if the student ID is not null
            if (currentStudent.Sid != null)
            {
                // Check if the student has taken each exam
                ExamsWithTakenStatus = exams.Select(exam =>
                {
                    var hasTaken = _context.Student_Quiz
                        .Any(sq => sq.Sid == currentStudent.Sid && sq.Qid == exam.Qid);

                    return (exam, hasTaken);
                }).ToList();
            }
            else
            {
                // If the student ID is null, set the list to empty
                ExamsWithTakenStatus = new List<(Quiz, bool)>();
            }

            return Page();
        }

        public IActionResult OnPostTakeExam(int examId)
        {
            int questionIndex = 0;

            return RedirectToPage("/Students/TakeExam", new { id = examId, questionIndex });
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
}
