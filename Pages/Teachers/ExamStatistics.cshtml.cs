// ExamStatistics.cshtml.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Test.Me.models;

public class ExamStatisticsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public ExamStatisticsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public int ExamId { get; set; }
    public List<QuestionStatisticsViewModel> QuestionStatistics { get; set; }

    public IActionResult OnGet(int examId)
    {
        ExamId = examId;

        // Retrieve question statistics for the specified exam
        QuestionStatistics = _context.Question
            .Where(q => q.Qid == examId)
            .Select(q => new QuestionStatisticsViewModel
            {
                QuestionText = q.Qutext,
                CorrectAnswersCount = _context.Student_Question
                    .Count(sq => sq.Quid == q.Quid && sq.Answrestate == "right"),
                TotalStudentsCount = _context.Student_Question
                    .Count(sq => sq.Quid == q.Quid),
                WrongAnswersCount = _context.Student_Question
                    .Count(sq => sq.Quid == q.Quid && sq.Answrestate == "Wrong"),
            })
            .ToList();

        return Page();
    }
}

public class QuestionStatisticsViewModel
{
    public String QuestionText { get; set; }
    public int CorrectAnswersCount { get; set; }
    public int TotalStudentsCount { get; set; }
    public int WrongAnswersCount { get; set; }

}
