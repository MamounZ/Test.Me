// AddQuestion.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using Test.Me.models;

namespace Test.Me.Pages
{
    public class AddQuestionModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AddQuestionModel> _logger;
        private readonly ApplicationDbContext _context;

        [BindProperty(SupportsGet = true)]
        public int? Qid { get; set; }

        public AddQuestionModel(IConfiguration configuration, ILogger<AddQuestionModel> logger, ApplicationDbContext context)
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

        public IActionResult OnPostMultipleChoice(string questionText, string correctAnswer, int mark)
        {
            return AddQuestion(true, questionText, correctAnswer, mark);
        }

        public IActionResult OnPostSimpleAnswer(string questionText, string correctAnswer, int mark)
        {
            return AddQuestion(false, questionText, correctAnswer, mark);
        }

        private IActionResult AddQuestion(bool isMultipleChoice, string questionText, string correctAnswer, int mark)
        {
            try
            {
                // Get QuizId from session
                var quizId = HttpContext.Session.GetInt32("Qid");

                if (quizId != null)
                {
                    // Create a new Question instance and set its properties
                    var question = new Question
                    {
                        Qid = quizId.Value,
                        Qutext = questionText,
                        Qurightanswer = correctAnswer,
                        Qumark = mark,
                        Qutype = isMultipleChoice ,
                    };

                    if (isMultipleChoice)
                    {
                        question.Firstop = Request.Form["firstop"];
                        question.Secondop = Request.Form["secondop"];
                        question.Thirdop = Request.Form["thirdop"];
                        question.Fourthop = Request.Form["fourthop"];
                    }

                    // Save the question to the database
                    _context.Question.Add(question);
                    _context.SaveChanges();

                    // Redirect to the same page to add more questions
                    return RedirectToPage("/Teachers/AddQuestion");
                }
                else
                {
                    // Quiz not selected
                    return RedirectToPage("/Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding a new question.");
                // Handle the error and return an appropriate response
                return Page();
            }
        }
    }
}
